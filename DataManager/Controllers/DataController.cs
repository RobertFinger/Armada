using Microsoft.AspNetCore.Mvc;
using Models.Models;


namespace DataManager.Controllers
{
    public class DataController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DataController(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewPlayer(Player player)
        {
            if (ModelState.IsValid)
            {
                if (player.Id == Guid.Empty)
                {
                    player.Id = Guid.NewGuid();
                }

                _dbContext.Players.Add(player);
                _dbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(player);
        }

        // GET: DataController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DataController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DataController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DataController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DataController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DataController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
