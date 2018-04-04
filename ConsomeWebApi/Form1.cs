using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsomeWebApi.Classes;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ConsomeWebApi
{
    public partial class FrmConsomeWebApi : Form
    {
        List<Usuario> usuario = new List<Usuario>();
        int pagina = 0;
        int proximaPagina = 0;
        public FrmConsomeWebApi()
        {
            InitializeComponent();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            AddProduto();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            ConsultaUsuario();
        }

        public async Task ConsultaUsuario()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:52745/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/usuario/ConsultarUsuarios");

                if (response.IsSuccessStatusCode)
                {  //GET
                    var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                    usuario = JsonConvert.DeserializeObject<Usuario[]>(ProdutoJsonString).ToList();
                    PopulaTela(0);
                }
                pagina = usuario.Count;
                lblPage.Text = "1";
            }
        }

        private async void AddProduto()
        {
            var URI = "http://localhost:52745/api/usuario/CadastrarUsuario";
            Usuario usu = new Usuario();
            usu.nome = txtNome.Text;
            usu.codigo = Convert.ToInt32(txtID.Text);
            usu.login = txtLogin.Text;

            using (var client = new HttpClient())
            {
                var serializedProduto = JsonConvert.SerializeObject(usu);
                var content = new StringContent(serializedProduto, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(URI, content);
                MessageBox.Show(result.ToString());
            }

        }

        private void PopulaTela(int pagina)
        {
            txtID.Text = usuario[pagina].codigo.ToString();
            txtNome.Text = usuario[pagina].nome.ToString();
            txtLogin.Text = usuario[pagina].login.ToString();
        }

        private void NavegaPagina(int _pagina)
        {
            var totalPagina = pagina;

            if (_pagina == 1)
            {
                if(proximaPagina != (totalPagina - 1))
                {
                    proximaPagina = proximaPagina + 1;
                    PopulaTela(proximaPagina); 
                }
            }
            else if (_pagina == 2)
            {
                if (proximaPagina != 0)
                {
                    proximaPagina = proximaPagina - 1;
                    PopulaTela(proximaPagina);
                }
            }

            if (proximaPagina > 0 || proximaPagina == (totalPagina - 1))
            {
                proximaPagina = proximaPagina + _pagina;
            }

            if (proximaPagina >= 0)
            {
                if (proximaPagina >= totalPagina)
                {

                }
                else
                {
                    PopulaTela(proximaPagina);
                    lblPage.Text = Convert.ToString(proximaPagina + 1);
                }
            }

        }

        private void bntAnterior_Click(object sender, EventArgs e)
        {
            NavegaPagina(-1);
        }

        private void bntProximo_Click(object sender, EventArgs e)
        {
            NavegaPagina(+1);
        }
    }
}
