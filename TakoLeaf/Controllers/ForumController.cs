using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TakoLeaf.Data;
using TakoLeaf.Models;
using TakoLeaf.ViewModels;

namespace TakoLeaf.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private IDalForum dalForum;
        private int userId;
        public ForumController()
        {
            this.dalForum = new DalForum();
            
        }

        public ActionResult Sujets()
        {
            List<Sujet> sujets = dalForum.GetAllSujets().ToList();
            return View(sujets);
        }

        public ActionResult Sujet(int? id)
        {
            ForumViewModel fvm = null;
            if (!id.HasValue)
            {
                return View("Error");
            }
            fvm = new ForumViewModel()
            {
                Sujet = dalForum.GetSujet(id.Value),
                Posts = dalForum.GetPosts(id.Value),
                PostSignale = new PostSignale()
            };
            return View(fvm);
        }

        [HttpPost]
        public ActionResult Sujet(ForumViewModel fvm)
        {
            userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            DateTime now = DateTime.Now;
            Post post = new Post()
            {
                AdherentId = userId,
                CorpsPost = fvm.Post.CorpsPost,
                Date = now,
                SujetId = fvm.Sujet.Id
            };
            this.dalForum.CreationPost(post);
            return RedirectToAction("Sujet", new { id = fvm.Sujet.Id });
        }

        public ActionResult NouveauSujet()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NouveauSujet(ForumViewModel fvm)
        {
            userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            DateTime now = DateTime.Now;
            Sujet sujet = new Sujet()
            {
                Date = now,
                IdAdherent = userId,
                Titre = fvm.Sujet.Titre
            };
            this.dalForum.CreationSujet(sujet);
            Sujet s = this.dalForum.RechercheSujetParTitre(sujet.Titre);
            Post post = new Post()
            {
                AdherentId = userId,
                CorpsPost = fvm.Post.CorpsPost,
                Date = now,
                SujetId = s.Id
            };
            this.dalForum.CreationPost(post);
            return RedirectToAction("Sujets");
        }

        [HttpPost]
        public ActionResult Signaler(int Sujet_Id, int item_AdherentId, int item_Id, string PostSignale_Message)
        {
            userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string now = DateTime.Now.ToShortDateString();
            PostSignale postSignale = new PostSignale()
            {
                Date = now,
                Vu = false,
                Message = PostSignale_Message,
                AdherentSignaleId = item_AdherentId,
                AdherentSignalantId = userId,
                PostId = item_Id
            };
            this.dalForum.AjoutPostSignale(postSignale);
            return RedirectToAction("Sujet", new { id = Sujet_Id });
        }

        /*public ActionResult ModifierPost(int? id)
        {
            Post post = null;
            if(id.HasValue)
            {
                post = dalForum.Get1Post(id.Value);
                dalForum.ModificationPost(post);
                return View(post);
            }
            return View("Error");
        }

        [HttpPost]
        public ActionResult ModifierPost(Post post)
        {
            if (!ModelState.IsValid)
                return View(post);
            this.dalForum.ModificationPost(post);
            return RedirectToAction("Sujet", post.SujetId);
        }

        public ActionResult ModifierSujet(int? id)
        {
            Sujet sujet = null;
            if(id.HasValue)
            {
                sujet = dalForum.GetSujet(id.Value);
                dalForum.ModificationSujet(sujet);
                return View();
            }
            return View("Error");
        }

        [HttpPost]
        public ActionResult ModifierSujet(Sujet sujet)
        {
            if (!ModelState.IsValid)
                return View(sujet);
            this.dalForum.ModificationSujet(sujet);
            return RedirectToAction("Sujets");
        }

        public ActionResult SupprimerSujet(int? id)
        {
            Sujet sujet = null;
            if(id.HasValue)
            {
                sujet = dalForum.GetSujet(id.Value);
                dalForum.SuppressionSujet(sujet);
                return View("Sujets");
            }
            return View("Error");
        }

        public ActionResult SupprimerPost(int? id)
        {
            Post post = null;
            if(id.HasValue)
            {
                post = dalForum.Get1Post(id.Value);
                int idSujet = post.SujetId;
                dalForum.SuppressionPost(post);
                return View("Sujet", idSujet);
            }
            return View("Error");
        }*/
    }
}