﻿using BazarBoutique.Modelos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BazarBoutique.VistaModelos.CatalogoCursosViewModels
{
    public class CatalogoCursosViewModel : BaseViewModel
    {
        private int _rowHeigth;
        public int rowHeigth
        {
            get { return _rowHeigth; }
            set
            {
                _rowHeigth = value;
                RaisePropertyChanged("rowHeigth");
            }
        }


        public ObservableCollection<CategoriaModelo> Categorias { get; }
        public ObservableCollection<CursosModelo> Cursos { get; }

        public CatalogoCursosViewModel()
        {
            Categorias = new ObservableCollection<CategoriaModelo>();
            Cursos = new ObservableCollection<CursosModelo>();
            AgregandoDatosCatalogo();
            AgregandoDatosCursos();
            DefiniendoAltura();
        }
        private void DefiniendoAltura()
        {
            int Elementos = Cursos.Count;
            if (Elementos <= 1)
            {
                rowHeigth = 255;
            }
            else if (Elementos % 2 == 0)
            {
                rowHeigth = (Elementos / 2) * 255;
            }

            else
                rowHeigth = ((Elementos / 2) * 255) + 255 ;
        }
        private void AgregandoDatosCatalogo()
        {
            Categorias.Add(new CategoriaModelo
            {
                category_id = 1,
                category_tile = "Cursos de Juegos de Azar",
                is_enabled = true,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            });

            Categorias.Add(new CategoriaModelo
            {
                category_id = 2,
                category_tile = "Cursos de Informatica",
                is_enabled = true,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            });

            Categorias.Add(new CategoriaModelo
            {
                category_id = 3,
                category_tile = "Cursos de Audicion",
                category_ico = "wwww",
                is_enabled = true,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            });
        }

        private void AgregandoDatosCursos()
        {
            Cursos.Add(new CursosModelo
            {
                course_id = 1,
                course_title = "Curso de C#",
                course_photo = "",
                is_enabled = true,
                created_at = DateTime.Now
            });

            Cursos.Add(new CursosModelo
            {
                course_id = 2,
                course_title = "Curso de C++",
                course_photo = "",
                is_enabled = true,
                created_at = DateTime.Now
            });

            Cursos.Add(new CursosModelo
            {
                course_id = 2,
                course_title = "Curso de Javascript",
                course_photo = "",
                is_enabled = true,
                created_at = DateTime.Now
            });

            Cursos.Add(new CursosModelo
            {
                course_id = 5,
                course_title = "Curso de WordPress",
                course_photo = "",
                is_enabled = true,
                created_at = DateTime.Now
            });
        }

    }
}