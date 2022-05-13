﻿using BazarBoutique.Modelos;
using BazarBoutique.Services;
using BazarBoutique.Services.CategoriaServices;
using BazarBoutique.Services.UsuarioServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace BazarBoutique.VistaModelos.FiltrosViewModels
{
    public class SoloFiltroCategoriaViewModel : BaseViewModel
    {
        #region Campos
        private ObservableCollection<CategoriaModelo> categoriasLista;
        private readonly INavigation navigation;
        private readonly ContentPage page;
        private string _elementoBusqueda;
        ICategoryService ServicioCategoria = DependencyService.Get<ICategoryService>();
        #endregion

        #region Propiedades
        public string ElementoBusqueda
        {
            get
            {
                return _elementoBusqueda;
            }
            set
            {
                SetProperty(ref _elementoBusqueda, value);
                BuscandoElementoAsync(value);
            }
        }
        public Command GestoRefrescamientoCommand { get; }
        public Command<Uri> PaginaAnteriorCommand { get; set; }
        public Command<Uri> PaginaSiguienteCommand { get; set; }
        public Command<PaginaRedireccion> RedireccionPaginaCommand { get; set; }
        DataCategorias PaginaDatos = new DataCategorias();

        public ObservableCollection<CategoriaModelo> CatalogosCategorias
        {
            get => categoriasLista;
            set
            {
                categoriasLista = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PaginaRedireccion> _PaginasListadas;

        public ObservableCollection<PaginaRedireccion> PaginasListadas
        {
            get => _PaginasListadas;
            set
            {
                _PaginasListadas = value;
                OnPropertyChanged();
            }
        }

        //Propiedad para boton de regreso de pagina
        private Color _BackButtonColor;
        public Color BackButtonColor
        {
            get => _BackButtonColor;
            set
            {
                SetProperty(ref _BackButtonColor, value);
            }
        }

        //Propiedad para boton de pagina siguiente
        private Color _NextButtonColor;
        public Color NextButtonColor
        {
            get
            {
                return _NextButtonColor;
            }
            set
            {
                SetProperty(ref _NextButtonColor, value);
            }
        }

        private Uri _ParametroPaginaAnterior;

        public Uri ParametroPaginaAnterior
        {
            get => _ParametroPaginaAnterior;
            set
            {
                BackButtonColor = value == null ? Color.Gray : Color.Green;
                SetProperty(ref _ParametroPaginaAnterior, value);
            }
        }
        
        private Uri _ParametroPaginaSiguiente;

        public Uri ParametroPaginaSiguiente
        {
            get => _ParametroPaginaSiguiente;
            set
            {
                NextButtonColor = value == null ? Color.Gray : Color.Green;
                SetProperty(ref _ParametroPaginaSiguiente, value);
            }
        }

        private LayoutState _estadoCategoria;

        public LayoutState EstadoCategoria
        {
            get => _estadoCategoria;
            set => SetProperty(ref _estadoCategoria, value);
        }

        //Botones Filtros
        public Command<CategoriaModelo> CambiandoEstadoFiltroCommand { get; set; }

        SearchCourseFilters FiltrosRealizados = new SearchCourseFilters()
        {
            filters = new FiltrosModelo() { }
        };
        private Uri LinkPagina;
        #endregion

        public SoloFiltroCategoriaViewModel(INavigation navigation, ContentPage page)
        {
            this.navigation = navigation;
            this.page = page;

            GestoRefrescamientoCommand = new Command(RecargarCategorias);
            RedireccionPaginaCommand = new Command<PaginaRedireccion>(SeleccionandoPagina);
            PaginaAnteriorCommand = new Command<Uri>(PaginaPorBoton);
            PaginaSiguienteCommand = new Command<Uri>(PaginaPorBoton);
            CambiandoEstadoFiltroCommand = new Command<CategoriaModelo>(CambiandoEstadoFiltro);

            PaginasListadas = new ObservableCollection<PaginaRedireccion>();
            CatalogosCategorias = new ObservableCollection<CategoriaModelo>();

            LinkPagina = new Uri("https://monolith-stage.herokuapp.com/api/v1/categories/search");

            EstadoCategoria = LayoutState.Loading;
        }

        public async void OnAppearing()
        {
            FiltrosRealizados.paginate = 6;
            FiltrosRealizados.filters.withDisabled = false;

            await EstableciendoValoresDePagina(LinkPagina, FiltrosRealizados);
        }

        private async void BuscandoElementoAsync(string value)
        {
            FiltrosRealizados.filters.title = value;
            IsLoading = true;
            IsLoading = false;
        }
        private async void RecargarCategorias()
        {
            IsLoading = true;

            await EstableciendoValoresDePagina(LinkPagina, FiltrosRealizados);

            IsLoading = false;
        }

        private void CambiandoEstadoFiltro(CategoriaModelo obj)
        {

            var ElementoSeleccionado = new FiltroPorNombreId
            {
                CodigoFiltro = obj.id,
                NombreFiltro = obj.title,
                TipoFiltro = "categories"
            };

            //bool ElFiltroExiste = FiltrosAlmacenados.AlmacenamientoFiltros.Any(x => x == ElementoSeleccionado);

            bool Existe = FiltrosAlmacenados.AlmacenamientoFiltros.Any(x => x.CodigoFiltro == ElementoSeleccionado.CodigoFiltro);


            //Confirmar que el articulo no exista
            if (!Existe) {
                
                //Comprobar si esta habilitado o no
                if (obj.EstaSeleccionado)
                {
                    //Aqui se activa
                    FiltrosAlmacenados.AlmacenamientoFiltros.Add(ElementoSeleccionado);
                }
                else
                {
                    //Aqui se elimina
                    FiltrosAlmacenados.AlmacenamientoFiltros.Remove(ElementoSeleccionado);
                }

            }   
        }

        private async void SeleccionandoPagina(PaginaRedireccion obj)
        {
            if (PaginaDatos.pagination.current_page != obj.LinkPagina)
                await EstableciendoValoresDePagina(obj.LinkPagina, FiltrosRealizados);
        }

        private async void PaginaPorBoton(Uri uri)
        {
            await EstableciendoValoresDePagina(uri, FiltrosRealizados);
        }

        private async Task EstableciendoValoresDePagina(Uri link, SearchCourseFilters fillter)
        {
            EstadoCategoria = LayoutState.Loading;

            //IsBusy = true;

            if (link != null)
            {
                CatalogosCategorias.Clear();
                PaginaDatos = await ServicioCategoria.GetPaginacionCategoria(link, fillter);

                if (PaginaDatos.categories != null)
                {
                    CatalogosCategorias.Clear();
                    foreach (var arg in PaginaDatos.categories)
                    {
                        CatalogosCategorias.Add(arg);
                    }
                    RefrescarPaginaSeleccionada();
                    ParametroPaginaSiguiente = PaginaDatos.pagination.next_page;
                }
                else
                {
                    PaginaDatos.categories = new List<CategoriaModelo>();
                }
            }
            //IsBusy = true;
            EstadoCategoria = LayoutState.Success;

        }


        private void RefrescarPaginaSeleccionada()
        {
            PaginasListadas.Clear();
            for (int i = 0; i < PaginaDatos.pagination.total;)
            {
                i++;
                var Elemento = new PaginaRedireccion();
                Elemento.LinkPagina = new Uri("https://monolith-stage.herokuapp.com/api/v1/categories/search?page=" + i);
                Elemento.NumeroPagina = i;

                if (PaginaDatos.pagination.current_page == Elemento.LinkPagina)
                {
                    Elemento.PaginaSeleccionada = true;
                    if (i <= 1)
                    {
                        ParametroPaginaAnterior = null;
                    }
                    else
                    {
                        ParametroPaginaAnterior = new Uri("https://monolith-stage.herokuapp.com/api/v1/categories/search?page=" + (i - 1));
                    }
                }
                else
                {
                    Elemento.PaginaSeleccionada = false;
                }
                PaginasListadas.Add(Elemento);
            }
        }







    }
}
