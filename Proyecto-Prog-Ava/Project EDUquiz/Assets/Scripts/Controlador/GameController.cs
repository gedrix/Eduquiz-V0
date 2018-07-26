using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public Canvas ui_game;
	public Canvas ui_main;
	public Canvas ui_options;
	public Canvas ui_clasificacion;
	public TextMeshProUGUI txtNombreUsu;
	public TMP_InputField inputNombreUsu;
	public TextMeshProUGUI txtMsjOpcion;
	public TextMeshProUGUI txtNivel;
	public TextMeshProUGUI txtTiempoPregunta;
	public TextMeshProUGUI txtTiempoPartida;
	public TextMeshProUGUI txtClasificacion;
	public TextMeshProUGUI txtExperiencia;
	public TextMeshProUGUI txtPorcentajeExp;
	public TextMeshProUGUI txtTotalExperiencia;
	public Slider sliderExperiencia;
	public TextMeshProUGUI txtResumenNivel;
	public TextMeshProUGUI txtPregunta;
	public TextMeshProUGUI txtCreadaPor;
	public TextMeshProUGUI txtDificultad;
	public TextMeshProUGUI txtCategorias;
	public TextMeshProUGUI[] txtOpcion;
	public Animator[] animOpcion;
	public Button[] btnOpcion;
	public GameObject panelAccion; //para activarlo / desactivarlo por script
	public GameObject panelCerrarSesion; //para activarlo / desactivarlo por script
	public GameObject panelIrAlMenu; //para activarlo / desactivarlo por script
	public GameObject panelRevPregunta; //para activarlo / desactivarlo por script
	public GameObject panelResumenPartida; //para activarlo / desactivarlo por script
	public Animator animPanelAccion; //animacion de la ventana al darle clic a una opcion
	public TextMeshProUGUI txtMsjResultado; //muestra si el usuario respondio de manera correcta o no.
	public Animator animTiempoPregunta;
	public Animator animSugerirPreg;
	public TMP_InputField inputCrearPregunta;
	public TMP_Dropdown cbxCategoria;
	public TMP_Dropdown cbxDificultad;
	public TMP_InputField[] inputCrearOpcion;
	public TextMeshProUGUI txtMsjCrearPregunta;
	public TextMeshProUGUI txtContadorPreg; //contador que aparece para saber el numero de la pregunta actual
	public TextMeshProUGUI txtPregAcertadas;
	public TextMeshProUGUI txtRachaAciertos;
	public TextMeshProUGUI txtRachaPuntos;
	public TextMeshProUGUI txtBonusTiempo;
	//panel de carga
	public GameObject panelCargando; //para activarlo / desactivarlo por script
	public GameObject btnCancelarCarga; //boton que se habilita si ocurrio algun error al intentar recibir info del servidor
	public TextMeshProUGUI txtCargando;
	public TextMeshProUGUI txtCargaTitulo;
	private int conCargaTexto = 0;
	private bool banCarga = false;
	private float timerPregunta = 15.99f;
	private float timerPartida = 0f;
	private bool banTimerPregunta = false;
	private bool banTimerPartida = false;
	private int experienciaActual = 0;
	private int minPartida = 0;
	private int numAcertadas = 0; //contador de respuestas acertadas correctamente...
	private int numPregunta = 0; //contador de la pregunta actual que se muestra en partida
	private int numMaxPreguntas = 10; //el numero de preguntas que se extraera por partida 'clasica'
	private int puntosExp = 0; //puntos de experiencia ganados en esa partida
	private int puntosClasificacion = 0; //puntos ganados (Clasificacion) por partida
	private int racha = 0, rachaMax = 0, rachaPuntos = 0; //para almacenar la racha de las preguntas acertadas
	private int bonusTiempo = 0;
	Persona objPersona; //auxiliar para la info del usuario
	Pregunta objPregunta; //auxiliar para la creacion de las preguntas
	Preguntas preguntas; //las N preguntas aleatorias
	ControlNiveles controlNiveles = new ControlNiveles ();
	private List<Pregunta> listaPreguntas;
	private List<int> listaAcertadas = new List<int> (); //almacena los id's de las preguntas correctas
	void Awake () {
		//esta linea al parecer no funca en movil...
		Application.runInBackground = true; //la aplicacion se mantiene ejecutando en segundo plano (por los timers)
		btnCancelarCarga.SetActive (false); //inabilito el boton hasta ser llamado...
		string json = PlayerPrefs.GetString ("InfoJugador", "");
		if (json == "") {
			SceneManager.LoadScene ("login");
		} else {
			mostrarInfoJugador (json);
		}
		btnIrAlMenu ();
	}
	void Update () {
		//si... es necesario las tres condiciones para no bugear el control del tiempo...
		if (banTimerPregunta && !animPanelAccion.GetBool ("FadeIn")) { //esto da una advertencia rara...
			timerPregunta -= Time.deltaTime;
			txtTiempoPregunta.text = timerPregunta.ToString ("0");
			if (timerPregunta < 0) {
				accionOpciones (-1, "Ups, tiempo agotado"); //-1 es el por defecto para saber si no fue clic del usuario
			}
		}
		if (banTimerPartida) {
			timerPartida += Time.deltaTime;
		}
		if (panelCargando.activeSelf) {
			pantallaCargando ();
		} else {
			if (banCarga) {
				banCarga = false;
				//detengo la coroutina, para no sobrecargar la memoria...
				StopCoroutine ("tiempoCarga");
			}
		}
	}
	private void pantallaCargando () {
		if (banCarga) {
			banCarga = false;
			StartCoroutine ("tiempoCarga");
		}
	}
	IEnumerator tiempoCarga () {
		yield return new WaitForSeconds (0.25f);
		switch (conCargaTexto) {
			case 1:
				txtCargando.text = "Cargando";
				break;
			case 2:
				txtCargando.text = "Cargando.";
				break;
			case 3:
				txtCargando.text = "Cargando..";
				break;
			case 4:
				txtCargando.text = "Cargando...";
				conCargaTexto = 0;
				break;
		}
		conCargaTexto++; //incremento el contador
		banCarga = true;
	}
	void mostrarInfoJugador (string json) {
		print (json);
		objPersona = JsonUtility.FromJson<Persona> (json);
		txtNombreUsu.text = objPersona.nombre;
		inputNombreUsu.text = objPersona.nombre;
		txtNivel.text = controlNiveles.obtenerNivelUsuario (objPersona.experiencia) [0] + "";
	}
	public void btnJugar () {
		desactivarTodosCanvas ();
		listaPreguntas = new List<Pregunta> ();
		listaPreguntas.Clear ();
		listaAcertadas.Clear ();

		obtenerPreguntas ();
		habilitarBotones ();
		limpiarOpciones ();
		txtCargaTitulo.text = "Estamos seleccionando tus preguntas";
		//activar panel de 'cargando el juego'
		panelCargando.SetActive (true);
		panelRevPregunta.SetActive (false); //desactivado momentaneamente...
		panelResumenPartida.SetActive (false); //porsia...
		panelAccion.SetActive (false); //porsia...
		btnCancelarCarga.SetActive (false); //porsia...
		banCarga = true;
		//reiniciar el contador de las preguntas y acertadas
		numPregunta = 0;
		numAcertadas = 0;
		racha = 0;
		rachaMax = 0;
		rachaPuntos = 0;
		bonusTiempo = 0;
		experienciaActual = 0;
		puntosClasificacion = 0;
		//hago visible la vista del juego
		ui_game.enabled = true;
	}
	public void btnOpciones () {
		desactivarTodosCanvas ();
		txtMsjOpcion.text = "";
		inputNombreUsu.interactable = false;

		ui_options.enabled = true;
	}
	public void btnClasificacion () {
		desactivarTodosCanvas ();

		ui_clasificacion.enabled = true;
	}
	private void desactivarTodosCanvas () {
		ui_game.enabled = false;
		ui_main.enabled = false;
		ui_options.enabled = false;
		ui_clasificacion.enabled = false;
	}
	public void btnVerPregunta () {
		panelRevPregunta.SetActive (true);
		animPanelAccion.SetBool ("FadeIn", false); //oculto el panel de accion
	}
	public void btnCerrarSesion () {
		PlayerPrefs.DeleteAll (); //USAR CON CUIDADO...
		SceneManager.LoadScene ("login");
	}
	public void btnCancelarCS () {
		panelCerrarSesion.SetActive (false);
	}
	public void btnSalirCS () {
		panelCerrarSesion.SetActive (true);
	}
	public void btnHomeIAM () {
		panelIrAlMenu.SetActive (true);
	}
	public void btnCancelarIAM () {
		panelIrAlMenu.SetActive (false);
	}
	public void activarInputNombre () {
		inputNombreUsu.interactable = true;
		inputNombreUsu.Select ();
	}
	public void btnIrAlMenu () {
		desactivarTodosCanvas ();
		//tiempo pregunta
		animTiempoPregunta.SetBool ("is15s", false);
		banTimerPregunta = false;
		banTimerPartida = false;
		timerPregunta = 0f;
		//desactivacion de paneles
		panelAccion.SetActive (false);
		panelIrAlMenu.SetActive (false);
		panelCargando.SetActive (false); //porsia...
		panelRevPregunta.SetActive (false); // tbm porsia...
		panelResumenPartida.SetActive (false);

		ui_main.enabled = true;
	}
	public void btnSugerirPregunta () {
		animSugerirPreg.SetBool ("mostrarSugerir", true);
	}
	public void btnRegresarSugerir () {
		animSugerirPreg.SetBool ("mostrarSugerir", false);
	}
	public void btnOpcion1 () {
		//pregunto por el .opcion[0], porque la respuesta correcta siempre se ubicara en la posicion 0
		if (txtOpcion[0].text == listaPreguntas[numPregunta - 1].opcion[0]) {
			animOpcion[0].SetBool ("Correcta", true);
		} else
			animOpcion[0].SetBool ("Incorrecta", true);
		accionOpciones (0);
	}
	public void btnOpcion2 () {
		if (txtOpcion[1].text == listaPreguntas[numPregunta - 1].opcion[0]) {
			animOpcion[1].SetBool ("Correcta", true);
		} else
			animOpcion[1].SetBool ("Incorrecta", true);
		accionOpciones (1);
	}
	public void btnOpcion3 () {
		if (txtOpcion[2].text == listaPreguntas[numPregunta - 1].opcion[0]) {
			animOpcion[2].SetBool ("Correcta", true);
		} else
			animOpcion[2].SetBool ("Incorrecta", true);
		accionOpciones (2);
	}
	public void btnOpcion4 () {
		if (txtOpcion[3].text == listaPreguntas[numPregunta - 1].opcion[0]) {
			animOpcion[3].SetBool ("Correcta", true);
		} else
			animOpcion[3].SetBool ("Incorrecta", true);
		accionOpciones (3);
	}
	//Cambiar dde nombre a este metodo
	public void editarNombre () { //boton de aceptar en las opciones
		if (inputNombreUsu.text != "" && inputNombreUsu.text != objPersona.nombre) {
			Persona personaAux = new Persona (); //cada vez que se da al boton iniciar se crea un nuevo objeto persona
			personaAux.nombre = inputNombreUsu.text.Trim (); //quitando los espacios en blanco (L & R)

			string json = JsonUtility.ToJson (personaAux);
			string url = "https://eduquiza.azurewebsites.net/public/index.php/usuario/editar/" +
				objPersona.external_id;

			Dictionary<string, string> headers = new Dictionary<string, string> ();
			headers.Add ("Content-Type", "application/json; charset=utf-8");
			headers.Add ("token", objPersona.token);

			byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

			WWW www = new WWW (url, pData, headers);
			StartCoroutine (requestEditarNombre (www));
		} else {
			btnIrAlMenu ();
		}
	}
	IEnumerator requestEditarNombre (WWW www) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("No se ha encontrado ningun dato"))
				txtMsjOpcion.text = "Error:\n<color=red><i>No se encontraron datos, vuelva a iniciar sesion.</color>"; //error
			if (www.text.Contains ("La data no tiene el formato deseado"))
				txtMsjOpcion.text = "Error:\n<color=red><i>Ha ocurrido un error al enviar los datos al servidor, intente mas tarde.</color>"; //error
			if (www.text.Contains ("Operacion existosa")) {
				txtMsjOpcion.text = "<align=center><color=green>Se ha actualizado la informacion correctamente";
				verificarSesion ();
				btnIrAlMenu ();
			}
		} else
			txtMsjOpcion.text = "Error:\n<color=red>" + www.error + "</color>";
	}
	private void verificarSesion (string espera = "nada") {
		string json = PlayerPrefs.GetString ("InfoJugador", "");
		//necesario por el encryptamiento de las claves
		Persona auxJson = JsonUtility.FromJson<Persona> (json);
		auxJson.clave = PlayerPrefs.GetString ("InfoClave", "");
		json = JsonUtility.ToJson (auxJson);
		string url = "https://eduquiza.azurewebsites.net/public/index.php/login";

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestSesion (www, espera));
	}
	IEnumerator requestSesion (WWW www, string espera) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("Operacion existosa")) {
				objPersona = JsonUtility.FromJson<Persona> (www.text);
				PlayerPrefs.SetString ("InfoJugador", www.text);
				mostrarInfoJugador (www.text);
				if (espera == "actualizarInfoUsuario") {
					actualizarInfoNivelUsuario ();
				}
			} else {
				//si no pudo encontrar la info del jugador, borro los datos locales y vuelvo a pedir iniciar sesion
				btnCerrarSesion ();
			}
		} else {
			btnCerrarSesion ();
		}
	}
	private void obtenerPreguntas () {
		objPregunta = new Pregunta ();
		objPregunta.dificultad = obtenerDificultad (); // esto obtendra la dificultad de acuerdo al nivel...
		objPregunta.cantidad = numMaxPreguntas; //obtengo el numero de preguntas seleccionado

		string json = JsonUtility.ToJson (objPregunta);

		//string url = "localhost:80/Proyecto-Prog-Ava/Juego/public/index.php/preguntas/ramdon"; //ruta local
		string url = "https://eduquiza.azurewebsites.net/public/index.php/preguntas/ramdon"; //ruta servidor

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");
		headers.Add ("token", objPersona.token);

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestPreguntas (www));
	}
	IEnumerator requestPreguntas (WWW www) {
		yield return www;
		btnCancelarCarga.SetActive (true);
		if (www.text.Contains ("Operacion existosa")) {
			btnCancelarCarga.SetActive (false);
			preguntas = JsonUtility.FromJson<Preguntas> (www.text);
			for (int i = 0; i < preguntas.preguntas.Length; i++) {
				Pregunta obId = new Pregunta ();
				obId.id = preguntas.ids[i];
				string jsonOp = JsonUtility.ToJson (obId);
				//string urlOp = "localhost:80/Proyecto-Prog-Ava/Juego/public/index.php/pregunta/obtenerPreguntaId"; //ruta local
				string urlOp = "https://eduquiza.azurewebsites.net/public/index.php/preguntas/obtenerPreguntaId"; //ruta servidor
				Dictionary<string, string> headersOp = new Dictionary<string, string> ();
				headersOp.Add ("Content-Type", "application/json; charset=utf-8");
				headersOp.Add ("token", objPersona.token);

				byte[] pData = System.Text.Encoding.UTF8.GetBytes (jsonOp.ToCharArray ());
				WWW wwwObPreg = new WWW (urlOp, pData, headersOp);

				yield return wwwObPreg;

				if (wwwObPreg.text.Contains ("Operacion existosa")) {
					btnCancelarCarga.SetActive (false);
					Pregunta auxOpciones = new Pregunta ();
					auxOpciones = JsonUtility.FromJson<Pregunta> (wwwObPreg.text);
					auxOpciones.pregunta = preguntas.preguntas[i];
					auxOpciones.dificultad = preguntas.dificultad[i];
					listaPreguntas.Add (auxOpciones);
				} else {
					btnCancelarCarga.SetActive (true);
					banCarga = false;
					StopCoroutine ("tiempoCarga");
					txtCargaTitulo.text = "Ha ocurrido un error, comprueba tu conexion a internet.\n" +
						"Detalles: " + wwwObPreg.text;
				}
			}
			yield return new WaitForSeconds (.1f);
			siguientePregunta ();
			Handheld.Vibrate (); //vibrar el movil... (avisando que ya se cargaron las preguntas)
		} else {
			print ("Error: " + www.error);
			banCarga = false;
			StopCoroutine ("tiempoCarga");
			txtCargaTitulo.text = "<color=red>Error:\n</color>" + www.error;
		}
	}
	public void siguientePregunta () {
		if (numPregunta < numMaxPreguntas) {
			limpiarOpciones (); //se limpian las opciones y pregunta
			//luego de obtener y mostrar la pregunta, habilito el timer y lo reinicio(por pregunta)
			timerPregunta = 15f; //esto deberia modificarse de acuerdo a la dificultad...
			//titulo pregunta
			txtPregunta.text = listaPreguntas[numPregunta].pregunta;
			//definir categorias
			if (listaPreguntas[numPregunta].categoria.Length != 1) {
				for (int i = 0; i < listaPreguntas[numPregunta].categoria.Length; i++) {
					if (i > 0)
						txtCategorias.text += ", " + listaPreguntas[numPregunta].categoria[i];
					else
						txtCategorias.text = listaPreguntas[numPregunta].categoria[i];
				}
			} else {
				txtCategorias.text = listaPreguntas[numPregunta].categoria[0];
			}
			//aleatoriedad de las opciones
			List<int> nums = new List<int> ();
			int nOp = Random.Range (0, 4);
			txtOpcion[0].text = listaPreguntas[numPregunta].opcion[nOp];
			nums.Add (nOp);
			while (true) {
				nOp = Random.Range (0, 4);
				if (!nums.Contains (nOp)) {
					if (txtOpcion[1].text == "") {
						txtOpcion[1].text = listaPreguntas[numPregunta].opcion[nOp];
						nums.Add (nOp);
						continue;
					}
					if (txtOpcion[2].text == "") {
						txtOpcion[2].text = listaPreguntas[numPregunta].opcion[nOp];
						nums.Add (nOp);
						continue;
					}
					if (txtOpcion[3].text == "") {
						txtOpcion[3].text = listaPreguntas[numPregunta].opcion[nOp];
						break;
					}
				}
			}
			//cargo la dificultad actual de la pregunta y quien la creó
			txtDificultad.text = listaPreguntas[numPregunta].dificultad;
			txtCreadaPor.text = "<b>Creada por:</b> " + listaPreguntas[numPregunta].creadaPor;
			//desactivo el panel de 'carga'
			panelCargando.SetActive (false);
			banCarga = false;
			StopCoroutine ("tiempoCarga");
			//al final incremento en 1 la el contador de las preguntas
			numPregunta++;
			txtContadorPreg.text = numPregunta.ToString ("0") + " / " + numMaxPreguntas.ToString ("0"); //esto puede cambiar mas adelante...
			banTimerPregunta = true; //activo el tiempo por pregunta
			banTimerPartida = true; //activo el tiempo de la partida actual
			animTiempoPregunta.SetBool ("is15s", true);
		} else { //En este caso quiere decir que ya completo las preguntas y debo mostrar la pantalla de completado
			banTimerPartida = false;
			panelCargando.SetActive (true);
			banCarga = true;
			txtCargaTitulo.text = "Comprobando respuestas";
			//contabilizar experiencia, nivel, etc
			//actualizarInfoNivelUsuario ();
			contabilizarPuntosExperiencia ();
		}
	}
	public void contabilizarPuntosExperiencia () {
		experienciaActual = 0;
		puntosClasificacion = 0; //porsia...
		for (int i = 0; i < listaAcertadas.Count; i++) {
			int[] infoPuntaje = controlNiveles.obtenerPuntosPorDificultad (objPersona.nivel,
				listaPreguntas[listaAcertadas[i]].dificultad);
			experienciaActual += infoPuntaje[0];
			puntosClasificacion += infoPuntaje[1];
		}
		//antes de enviar, tambien calculo la racha de aciertos y la incremento
		if (rachaMax == 2)
			rachaPuntos = Random.Range (0, 3); //incremento un aleatorio de 0 a 2 puntos
		if (rachaMax >= 3 && racha <= 5)
			rachaPuntos = Random.Range (5, 11); //incremento un aleatorio de 5 a 10 puntos
		if (rachaMax >= 6 && racha <= 8)
			rachaPuntos = Random.Range (10, 26); //incremento un aleatorio de 10 a 25 puntos
		if (rachaMax >= 9)
			rachaPuntos = Random.Range (25, 51); //incremento un aleatorio de 25 a 50 puntos
		txtExperiencia.text = experienciaActual + " Pts";
		experienciaActual += rachaPuntos; //incremento el bonus a la experiencia
		//calcular el bonus de tiempo
		minPartida = 0; //controla los minutos
		for (int i = 0; i < 10; i++) {
			if (timerPartida >= 60f) {
				timerPartida -= 60f;
				minPartida++;
			} else {
				break;
			}
		}
		//control del bonus de tiempo
		if (numAcertadas >= 3) //para acceder al bonus de tiempo es necesario tener minimo 3 preguntas acertadas
		{
			if (minPartida < 1) // si es menos de 1 minuto (aplico un bonus de tiempo)
			{
				bonusTiempo = Random.Range (10, 21); //el bonus sera aleatorio entre 10 y 20 puntos
				if (timerPartida <= 30f) // si es menos de 30 segundos (bonus de tiempo mayor)
					bonusTiempo = Random.Range (20, 36); //el bonus sera aleatorio entre 20 y 35 puntos
				if (numAcertadas >= 5) //para acceder al bonus mayor se necesita minimo 5 acertadas
				{
					if (timerPartida <= 15f) // si es menos de 15 segundos (el mejor bonus de tiempo)
						bonusTiempo = Random.Range (35, 51); //el bonus sera aleatorio entre 35 y 50 puntos	
				}
			} else if (minPartida == 1) {
				if (timerPartida <= 30f) // si es menos de 1:30 segundos (bonus de tiempo mayor)
					bonusTiempo = Random.Range (0, 11); //el bonus sera aleatorio entre 0 y 10 puntos
			}
		}
		verificarSesion ("actualizarInfoUsuario");
	}
	private void actualizarInfoNivelUsuario () {
		puntosExp = experienciaActual + bonusTiempo;
		objPersona.experiencia += puntosExp; //incremento la experiencia ganada	
		objPersona.nivel = controlNiveles.obtenerNivelUsuario (objPersona.experiencia) [0];
		objPersona.puntaje += puntosClasificacion; //incremento el puntaje

		string json = JsonUtility.ToJson (objPersona);

		//string url = "localhost:80/Proyecto-Prog-Ava/Juego/public/index.php/usuario/actualizarinfo/" +
		//	objPersona.external_id; //ruta local
		string url = "https://eduquiza.azurewebsites.net/public/index.php/usuario/actualizarinfo/" +
			objPersona.external_id; //ruta servidor

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");
		headers.Add ("token", objPersona.token);

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestInfoNivelUser (www));
	}
	IEnumerator requestInfoNivelUser (WWW www) {
		yield return www;
		btnCancelarCarga.SetActive (true);
		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("Operacion existosa")) {
				btnCancelarCarga.SetActive (false);
				verificarSesion ();
				//desactivo el panel de 'carga'
				panelCargando.SetActive (false);
				banCarga = false;
				StopCoroutine ("tiempoCarga");
				partidaClasicaTerminada ();
			} else {
				banCarga = false;
				StopCoroutine ("tiempoCarga");
				txtCargaTitulo.text = "Ha ocurrido un error, comprueba tu conexion a internet.\n" +
					"Detalles: " + www.text;
			}
		} else {
			print ("Error: " + www.error);
			banCarga = false;
			StopCoroutine ("tiempoCarga");
			txtCargaTitulo.text = "<color=red>Error:\n</color>" + www.error;
		}
	}
	private void partidaClasicaTerminada () {
		numPregunta = 0; //reinicio el contador
		panelResumenPartida.SetActive (true);

		txtTiempoPartida.text = minPartida + timerPartida.ToString (":00") + "s";
		txtPregAcertadas.text = numAcertadas + " de " + numMaxPreguntas;
		txtRachaAciertos.text = rachaMax + "";
		txtBonusTiempo.text = bonusTiempo + " Pts";
		txtRachaPuntos.text = rachaPuntos + " Pts";
		txtResumenNivel.text = "Nivel " + objPersona.nivel;
		int[] infoNivel = controlNiveles.obtenerNivelUsuario (objPersona.experiencia);

		StartCoroutine ("contarPuntaje");
		StartCoroutine (contarPorcentaje (infoNivel));
	}
	IEnumerator contarPuntaje () {
		for (int i = 0; i < puntosExp; i += 3) {
			yield return new WaitForSeconds (0.02f);
			txtTotalExperiencia.text = i + " Pts";
		}
		txtTotalExperiencia.text = puntosExp + " Pts";
	}
	IEnumerator contarPorcentaje (int[] infoNivel) {
		for (int i = 0; i < infoNivel[1]; i += 2) {
			yield return new WaitForSeconds (0.02f);
			txtPorcentajeExp.text = i + "%";
			sliderExperiencia.value = i;
		}
		txtPorcentajeExp.text = infoNivel[1] + "%";
		sliderExperiencia.value = infoNivel[1];
	}
	public void btnCrearPregunta () {
		if (inputCrearPregunta.text != "" && inputCrearOpcion[0].text != "" && inputCrearOpcion[1].text != "" &&
			inputCrearOpcion[2].text != "" && inputCrearOpcion[3].text != "") {

			Pregunta objPregunta = new Pregunta (); //cada vez que se da al boton iniciar se crea un nuevo objeto persona
			objPregunta.pregunta = inputCrearPregunta.text;
			objPregunta.dificultad = cbxDificultad.options[cbxDificultad.value].text;
			objPregunta.categoria = new string[1]; //definir el tamaño del arreglo para que no te joda...
			objPregunta.categoria[0] = cbxCategoria.options[cbxCategoria.value].text;
			for (int i = 0; i < 4; i++) {
				objPregunta.opcion[i] = inputCrearOpcion[i].text;
			}

			string jsonSg = JsonUtility.ToJson (objPregunta);

			string url = "https://eduquiza.azurewebsites.net/public/index.php/preguntas/registro/" +
				objPersona.external_id;

			Dictionary<string, string> headers = new Dictionary<string, string> ();
			headers.Add ("Content-Type", "application/json; charset=utf-8");
			headers.Add ("token", objPersona.token);

			byte[] pData = System.Text.Encoding.UTF8.GetBytes (jsonSg.ToCharArray ());

			WWW www = new WWW (url, pData, headers);
			txtMsjCrearPregunta.text = "Espere...";
			StartCoroutine (requestCrearPregunta (www));
		} else {
			txtMsjCrearPregunta.text = "Complete todos los campos";
		}
	}
	IEnumerator requestCrearPregunta (WWW www) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("Faltan datos en formulario"))
				txtMsjCrearPregunta.text = "Error:\n<color=red><i>Llene todos los campos</color>"; //error
			if (www.text.Contains ("La data no tiene el formato deseado"))
				txtMsjCrearPregunta.text = "Error:\n<color=red><i>Ha ocurrido un error al enviar los datos al servidor, intente mas tarde.</color>"; //error
			if (www.text.Contains ("Correo ya registrado"))
				txtMsjCrearPregunta.text = "Error:\n<color=red><i>Este correo esta actualmente en uso</color>"; //error
			if (www.text.Contains ("Operacion existosa")) {
				txtMsjCrearPregunta.text = "<align=center><color=green>Se ha registrado correctamente"; //text of success
			}
		} else
			txtMsjCrearPregunta.text = "Error:\n<color=red>" + www.error + "</color>"; //error
	}
	private void limpiarOpciones () {
		txtPregunta.text = "";
		for (int i = 0; i < 4; i++) {
			txtOpcion[i].text = "";
		}
		txtMsjResultado.text = "";
	}
	private void habilitarBotones () {
		for (int i = 0; i < 4; i++) {
			btnOpcion[i].interactable = true;
			animOpcion[i].SetBool ("Correcta", false);
			animOpcion[i].SetBool ("Incorrecta", false);
		}
	}
	private void accionOpciones (int n = -1, string str = "¡Has fallado!") {
		animTiempoPregunta.SetBool ("is15s", false);
		banTimerPregunta = false;
		banTimerPartida = false;
		for (int i = 0; i < 4; i++) {
			btnOpcion[i].interactable = false;
		}
		panelAccion.SetActive (true);
		if (n != -1) { //si se seleciono una opcion... sea correcta o no
			if (animOpcion[n].GetBool ("Correcta")) { //si el usuario encontro la correcta
				txtMsjResultado.text = "¡Correcto!";
				racha++; //incremento la racha de aciertos
				numAcertadas++; //incremento los aciertos
				listaAcertadas.Add (numPregunta - 1); //almaceno los id's de las preguntas acertadas
				if (rachaMax < racha)
					rachaMax = racha;
			} else { //incorrecta y mostrar al usuario la correcta
				racha = 0; //pierde la racha
				txtMsjResultado.text = str;
				buscarCorrecta ();
			}
		} else { //busca la respuesta correcta, si se acabo el tiempo
			racha = 0; //pierde la racha
			txtMsjResultado.text = str;
			buscarCorrecta ();
		}
		animPanelAccion.SetBool ("FadeIn", true);
	}
	private void buscarCorrecta () {
		for (int i = 0; i < 4; i++) {
			if (txtOpcion[i].text == listaPreguntas[numPregunta - 1].opcion[0]) {
				animOpcion[i].SetBool ("Correcta", true);
				break;
			}
		}
	}
	public void btnContinuar () {
		animPanelAccion.SetBool ("FadeIn", false);
		panelRevPregunta.SetActive (false);
		siguientePregunta ();
		habilitarBotones ();
	}
	private string obtenerDificultad () {
		string strDefault = "Facil";

		if (objPersona.nivel > 0)
			strDefault = "Facil";
		if (objPersona.nivel > 5)
			strDefault = "Normal";
		if (objPersona.nivel > 10)
			strDefault = "Dificil";
		if (objPersona.nivel > 15)
			strDefault = "Muy Dificil";
		if (objPersona.nivel > 20)
			strDefault = "Maestro";

		return strDefault;
	}
}