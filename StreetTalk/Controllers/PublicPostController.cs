﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using F23.StringSimilarity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreetTalk.Models;
using StreetTalk.Data;
using StreetTalk.Services;
using System.ComponentModel.DataAnnotations;
using StreetTalk.Dtos;

namespace StreetTalk.Controllers
{

    [Authorize]
    public class PublicPostController : BaseController
    {
        private readonly IPostService postService;
        private readonly IUserService userService;
        private readonly IFileUploadService fileUploadService;

        public PublicPostController(StreetTalkContext context, IPostService postService, IUserService userService, IFileUploadService fileUploadService) : base(context)
        {
            this.postService = postService;
            this.userService = userService;
            this.fileUploadService = fileUploadService;
        }

        public IActionResult Index(PublicPostListFilters filters, int page = 1, bool createSuccess = false)
        {
            ViewData["createSuccess"] = createSuccess;
            var perPage = 10;
            var skip = Math.Max(page - 1, 0) * perPage;
            var posts = Db.PublicPost.Where(p => !p.Closed || filters.ShowClosedPosts);

            if (filters.OnlyLikedPosts)
            {
                posts = MyLikedPosts(posts);
            }

            posts = DateRange(posts, filters);
            posts = ZoekFilter(posts, filters.ZoekFilter);

            switch (filters.SorteerOptie)
            {
                case "likes": posts = SorteerOpLikes(posts); break;
                case "views": posts = SorteerOpViews(posts); break;
                default: posts = SorteerOpDatum(posts); break;
            }

            int maxPages;
            if (posts.Count() % perPage != 0)
            {
                maxPages = (posts.Count() / perPage) + 1;
            }
            else maxPages = (posts.Count() / perPage);

            ViewData["maxPages"] = maxPages;


            posts = posts.Skip(skip).Take(perPage);

            var publicPostsWithLikes = posts.Select(a =>
                new PublicPostWithExtraData
                {
                    Post = a,
                    Liked = a.Likes.Any(b => b.UserId == userService.GetCurrentlyLoggedInUser().Id),
                    Reported = a.Reports.Any(b => b.UserId == userService.GetCurrentlyLoggedInUser().Id)
                }
            ).ToList();

            var viewModelData = new PublicPostViewModel
            {
                Posts = publicPostsWithLikes,
                Filters = filters,
                PreviousPage = Math.Max(page - 1, 1), //TODO: Make a paginated list class
                NextPage = page + 1
            };


            return View(viewModelData);
        }

        private IQueryable<PublicPost> SorteerOpLikes(IEnumerable<PublicPost> posts)
        {
            return posts.OrderByDescending(p => p.Likes.Count).AsQueryable();
        }
        private IQueryable<PublicPost> SorteerOpViews(IEnumerable<PublicPost> posts)
        {
            return posts.OrderByDescending(p => p.Views.Count).AsQueryable();
        }
        private IQueryable<PublicPost> SorteerOpDatum(IEnumerable<PublicPost> posts)
        {
            return posts.OrderByDescending(p => p.CreatedAt).AsQueryable();
        }

        private IQueryable<PublicPost> ZoekFilter(IEnumerable<PublicPost> post, string zoekterm)
        {
            if (!string.IsNullOrEmpty(zoekterm))
            {
                return post
                    .Where(s => s.Title.ToUpper().Contains(zoekterm.ToUpper()))
                    .AsQueryable();
            }
            
            return post.AsQueryable();
        }

        private IQueryable<PublicPost> MyLikedPosts(IEnumerable<PublicPost> post)
        {
            var user = userService.GetCurrentlyLoggedInUser();
            return post
                .ToList()
                .Where(p => p.Likes.Any(b => b.UserId == user.Id))
                .AsQueryable();
        }

        private IQueryable<PublicPost> DateRange(IEnumerable<PublicPost> posts, PublicPostListFilters filters)
        {
            if (!(filters.StartTime < new DateTime (1950, 01, 01)) && !(filters.EndTime < new DateTime(1950, 01, 01)))
            {
                return posts
                    .Where(p => p.ToDate(p.CreatedAt) <= filters.EndTime.Date && p.ToDate(p.CreatedAt) >= filters.StartTime.Date)
                    .AsQueryable();
            }
            return posts.AsQueryable();
        }

        public IActionResult Create()
        {
            IEnumerable<PostCategory> categories = Db.PostCategory.ToList();
            ViewData["categories"] = categories;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PublicPost post)
        {
            IEnumerable<PostCategory> categories = Db.PostCategory.ToList();
            ViewData["categories"] = categories;
            
            if (!ModelState.IsValid) return View(post);

            //Photo upload
            if (post.UploadedPhoto != null && post.UploadedPhoto.Length > 0)
            {
                var postPhoto = fileUploadService.HandlePostPhotoUpload(post.UploadedPhoto, post.UploadedPhotoIsSensitive);
                post.Photo = postPhoto;
            }

            var user = userService.GetCurrentlyLoggedInUser();
            post.UserId = user?.Id;
            user?.Posts.Add(post);

            await Db.SaveChangesAsync();

            return RedirectToAction("Index", new { createSuccess = true });
        }

        public IActionResult Post(int id)
        {
            ViewData["CurrentUserId"] = userService.GetCurrentlyLoggedInUser()?.Id;
            var post = postService.GetPublicPostById(id);
            var user = userService.GetCurrentlyLoggedInUser();
            
            if(post == null)
                return BadRequest("Post is null");
            
            if(user == null)
                return BadRequest("User is null");
            
            if (!postService.UserViewedPost(user.Id, post))
                postService.AddView(user.Id, post);
            
            ViewData["ViewAction"] = user.Id == post.UserId;
            
            return View(post);
        }

        [HttpPost]
        public IActionResult PostLike(int id)
        {
            if (userService.GetCurrentlyLoggedInUser() == null)
                return Json(new PostJsonResult { Succes = false, Error = "U moet eerst inloggen" });

            try
            {
                var post = postService.GetPublicPostById(id);
                postService.ToggleLikeForPost(post, userService.GetCurrentlyLoggedInUser()?.Id);

                return Json(new PostJsonResult { Succes = true, NewLikes = post.Likes.Count() });
            }
            catch
            {
                return Json(new PostJsonResult { Succes = false, Error = "Wijziging kon niet worden opgeslagen" });
            }
        }

        [HttpPost]
        public IActionResult PostReport(int id)
        {
            if (userService.GetCurrentlyLoggedInUser() == null)
                return Json(new PostJsonResult { Succes = false, Error = "U moet eerst inloggen" });

            try
            {
                var post = postService.GetPublicPostById(id);

                if (postService.UserReportedPost(post, userService.GetCurrentlyLoggedInUser()?.Id))
                    return Json(new PostJsonResult { Succes = false, Error = "Je hebt deze post al gerapporteerd" });

                postService.AddReportForPost(post, userService.GetCurrentlyLoggedInUser()?.Id);
                return Json(new PostJsonResult { Succes = true });
            }
            catch
            {
                return Json(new PostJsonResult { Succes = false, Error = "Wijziging kon niet worden opgeslagen" });
            }
        }

        [HttpPost]
        public IActionResult PostComment(int id, string commentContent)
        {
            if (string.IsNullOrEmpty(commentContent)) return RedirectToAction("Post", new {id});

            var postedComment = new Comment
            {
                Content = commentContent,
                AuthorId = userService.GetCurrentlyLoggedInUser()?.Id,
                PostId = id
            };
            
            postService.GetPublicPostById(id).Comments.Add(postedComment);
            Db.SaveChanges();


            return RedirectToAction("Post", new { id });
        }

        [Authorize(Roles = "Moderator, Administrator")]
        [HttpGet]
        public IActionResult CensorComment(int id, int commentId)
        {
            ViewData["PublicPostId"] = id;
            return View(postService.GetPublicPostById(id).Comments.Single(c => c.Id == commentId));
        }

        [Authorize(Roles = "Moderator, Administrator")]
        [HttpPost]
        public IActionResult CensorComment(int commentId, int id, string newContent)
        {
            postService.GetPublicPostById(id).Comments.Single(c => c.Id == commentId).Content = newContent;
            Db.SaveChanges();

            return RedirectToAction("Post", new { id });
        }

        public IActionResult Edit(int id)
        {
            return View(postService.GetPublicPostById(id));
        }

        [HttpPost]
        public IActionResult Edit(PublicPost post, int id)
        {
            var user = userService.GetCurrentlyLoggedInUser();

            if (postService.GetPublicPostById(id).UserId != user?.Id)
            {
                return RedirectToAction("Index");
            }

            var editedPost = postService.EditPostById(id, post);

            var context = new ValidationContext(editedPost);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(editedPost, context, validationResults, true);

            if (!isValid) return View(post);

            Db.SaveChanges();

            return RedirectToAction("Post", new { id = post.Id });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = userService.GetCurrentlyLoggedInUser();

            if (postService.GetPublicPostById(id).UserId != user?.Id)
            {
                return RedirectToAction("Index");
            }

            postService.DeletePostById(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Moderator")]
        public IActionResult Close(int id)
        {
            var post = postService.GetPublicPostById(id);
            post.Closed = true;
            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CheckPostTitleSimilarity(string title)
        {
            if (title.IsNullOrEmpty()) return BadRequest();
            var lcs = new MetricLCS();
            
            foreach (var recentTitle in postService.GetRecentTitles())
            {
                if (lcs.Distance(title, recentTitle) <= 0.5)
                    return Ok(new {Title = recentTitle});
            }

            return NoContent();
        }

    }
}