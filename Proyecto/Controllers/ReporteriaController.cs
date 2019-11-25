﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;
using System.Data.SqlClient;
using System.Windows;

namespace Proyecto.Controllers
{
    public class ReporteriaController : Controller
    {
        public string usuario = "";
        public string cedula = "";
        public string proy = "";

        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
        Proyecto.Controllers.HabilidadesController habController = new Proyecto.Controllers.HabilidadesController();
        // GET: Reporteria
        public ActionResult Index()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            //Listas que se usan para el despliegue de los proyectos y habilidades
            List<Proyecto.Models.Proyecto> proyectos = new ProyectoController().gettProyectos(usuario, cedula);
            List<string> habilidades = new HabilidadesController().getHabilidades();

            //Guardan temporalmente los datos
            TempData["proyectos"] = proyectos;
            TempData.Keep();

            return View();

        }


        // DESARROLLADOR POR CONOCIMIENTO
        /* consulta para obtener el numero de empleador por conocimiento y el promedio de tiempo trabajar en la empresa
         * @return lista 
         */
        public ActionResult DesarrolladoresPorConocimiento()
        {
                var item = (from habi in db.Habilidades
                            from emp in db.EmpleadoDesarrollador
                            where habi.cedulaEmpleadoPK_FK == emp.cedulaED
                            group new { emp, habi } by new { conocimientos = habi.conocimientos } into g
                            orderby g.Key.conocimientos ascending
                            select new { nombre = g.Key.conocimientos, cantDesa = g.Count(),  promedio = g.FirstOrDefault().emp.fechaInicio });
            TempData["Hab"] = habController.getHabilidades();

                List<string> datos = new List<string>();
                foreach (var dato in item)
                {
                    datos.Add(dato.nombre + " " + dato.cantDesa + " " + dato.promedio);
                }

                return View(datos);//retorna la vista
        }

        public ActionResult EstadoRequerimiento()
        {

            List<string> datosObtenidos = new List<string>();

            return View(datosObtenidos);//retorna la vista
        }

        [HttpPost]
        public ActionResult EstadoRequerimiento(string Proyecto)
        {
            if (usuario == "Cliente")
            { //si soy cliente puedo solamente ver  mis proyectos
                var obj = from a in db.Proyecto
                          where a.cedulaCliente == cedula
                          select a;

                return View(obj.Distinct().ToList());
            }

            ViewBag.Proy = Proyecto;

            var item = (from a in db.Requerimiento
                            from b in db.EmpleadoDesarrollador
                            where a.cedulaResponsable_FK == b.cedulaED
                            where a.nombreProyecto_FK == Proyecto
                            select new { nombreReq = a.nombre, estadoReq = a.estado, nombreDes = b.nombreED, apellido1Des = b.apellido1ED, apellido2Des = b.apellido2ED });

            if (Proyecto == "" || Proyecto == "Todos los Proyectos")
            {
                ViewBag.Proy = "Todos los Proyectos";
                item = (from a in db.Requerimiento
                        from b in db.EmpleadoDesarrollador
                        where a.cedulaResponsable_FK == b.cedulaED
                        select new { nombreReq = a.nombre, estadoReq = a.estado, nombreDes = b.nombreED, apellido1Des = b.apellido1ED, apellido2Des = b.apellido2ED });

            }

            List<string> datosObtenidos = new List<string>();
            foreach (var dato in item)
            {
                datosObtenidos.Add( dato.nombreReq + " " + dato.estadoReq + " " + dato.nombreDes + " " + dato.apellido1Des + " " + dato.apellido2Des);
            }

            return View(datosObtenidos);//retorna la vista
        }
        public SelectList getProyectos(String rol, String cedula)
        {
            return proyController.getProyectos(rol, cedula);
        }

       
    }
}


