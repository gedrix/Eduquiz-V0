using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions; //para utilizar expresiones regulares(validar email)
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Clase que controla metodos de inicio de sesion, registro y validacion del correo electronico
/// </summary>
public class SesionController : MonoBehaviour {
	public TMP_InputField inputCorreo; //otiene lo escrito en la caja de correo
	public TMP_InputField inputClave; //obtiene lo escrito en la caja de contraseña
	public TextMeshProUGUI txtMensaje; //muestra los mensajes de error
	public TextMeshProUGUI txtCargando; //texto para mostrar la carga sincrona de los datos obtenidos por request
	//animacion del registro
	public Animator animReg; //obtiene el componente <Animator> para controlar sus respectivas animaciones
	private Persona objPersona; //crea un objeto del modelo <Persona> auxiliar para almacenar la informacion del registro y sesion del usuario
	//datos de registro
	public TMP_InputField inputRegNombre; //caja de texto que obtiene el nombre del registro del usuario
	public TMP_InputField inputRegCorreo; //caja de texto que obtiene el correo del registro del usuario
	public TMP_InputField inputRegClave; //caja de texto que obtiene la clave del registro del usuario
	public TMP_InputField inputRegReClave; //caja de texto que obtiene el verificar-clave del registro del usuario
	public TextMeshProUGUI txtMensajeRegistrar; //muestra los mensajes de error del panel de registro
	public GameObject panelCargando; //para activar/desactivar el panel que se muestra al cargar los datos del servidor
	public GameObject btnAceptar; //boton auxiliar que se muestra en el panel de 'carga' para volver o cancelar la carga de datos
	private bool banCarga = false; //bandera para determinar si se muestra el estado del panel de 'carga'
	private int conCargaTexto = 0; //entero auxiliar que sirve de referencia para un mensaje en la pantalla de 'carga'

	/// <summary>
	/// Este metodo es el primero en iniciarse y se ejecuta solo una vez, necesario para referenciar
	/// componentes o definir el valor inicial de una variable, en este caso se desactiva el panel
	/// de 'carga'.
	/// </summary>
	private void Awake () {
		panelCargando.SetActive (false);
	}

	/// <summary>
	/// Se ejecuta una sola vez, justo despues del método Awake(), siempre y cuando el componente
	/// en el que se encuentre este activado.
	/// En este caso es utilizado para verificar la informacion del jugador (si ya se ha logueado antes)
	/// y convertir toda esa informacion a json, para que pueda ser enviada como 'Request' al servidor
	/// </summary>
	public void Start () {
		btnAceptar.SetActive (false);
		//verifico si la informacion del jugador ya se encuentra almacenada de manera local
		string json = PlayerPrefs.GetString ("InfoJugador", "");
		string clave = PlayerPrefs.GetString ("InfoClave", "");
		if (json != "") { //revisar
			Persona auxJson = JsonUtility.FromJson<Persona> (json);
			auxJson.clave = clave;
			json = JsonUtility.ToJson (auxJson);
			verificarSesion (json);
		}
	}

	/// <summary>
	/// Este método es ejecutado el numero de veces que fluyen los cuadros por segundo; un ejemplo
	/// simple es, si la aplicacion corre a 60fps(Frames per second) el metodo Update() se ejecutará
	/// 60 veces cada segundo...
	/// En este caso, este metodo se ha utilizado unicamente para refrescar la pantalla de carga,
	/// ejecutando una coroutina que sirve para 'dormir' la ejecucion secuencial N segundos o micro-
	/// segundos.
	/// </summary>
	void Update () {
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

	/// <summary>
	/// Un metodo publico que unicamente activa la animacion del boton registrar.
	/// </summary>
	/// <remarks>
	/// Es necesario que este método sea publico para que pueda ser referenciado mediante el editor
	/// de Unity
	/// </remarks>
	public void btnRegistrarse () {
		animReg.SetBool ("banRegistrar", true);
	}

	/// <summary>
	/// Un metodo publico que unicamente activa la animacion del boton regresar.
	/// </summary>
	/// <remarks>
	/// Es necesario que este método sea publico para que pueda ser referenciado mediante el editor
	/// de Unity
	/// </remarks>
	public void btnRegresar () {
		animReg.SetBool ("banRegistrar", false);
	}

	/// <summary>
	/// Método público que captura los valores de los 'InputField', verificando si los campos
	/// no estan vacios y si la claves ingresadas coinciden, además comprueba que el correo
	/// ingresado cumple con el formato deseado.
	/// Si todo esta correcto, transforma esta información en Json para su envio al servicio web
	/// mediante una coroutine, esta se encarga de esperar la respuesta del WS.
	/// </summary>
	/// <remarks>
	/// Es necesario que este método sea publico para que pueda ser referenciado mediante el editor
	/// de Unity
	/// </remarks>
	public void btnCrearCuenta () {
		if (inputRegNombre.text != "" && inputRegCorreo.text != "" && inputRegClave.text != "" &&
			inputRegReClave.text != "") {
			if (inputRegClave.text == inputRegReClave.text) { //si las claves coinciden
				objPersona = new Persona (); //cada vez que se da al boton iniciar se crea un nuevo objeto persona
				objPersona.nombre = inputRegNombre.text.Trim (); //quitando los espacios en blanco (L & R)
				objPersona.correo = inputRegCorreo.text.Trim (); //quitando los espacios en blanco (L & R)
				objPersona.clave = inputRegClave.text.Trim (); //quitando los espacios en blanco (L & R)

				if (!validarCorreo (objPersona.correo)) {
					txtMensajeRegistrar.text = "<color=red>Correo no válido";
					return;
				}

				string json = JsonUtility.ToJson (objPersona);
				print (json);

				//string url = "https://eduquiza.azurewebsites.net/public/index.php/usuario/registro";//server azure
				string url = "https://eduquiz.000webhostapp.com/public/index.php/usuario/registro"; //server free

				Dictionary<string, string> headers = new Dictionary<string, string> ();
				headers.Add ("Content-Type", "application/json; charset=utf-8");
				byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

				WWW www = new WWW (url, pData, headers);
				txtMensajeRegistrar.text = "Espere...";
				StartCoroutine (requestRegistro (www));
			} else {
				inputRegClave.text = "";
				inputRegReClave.text = "";
				txtMensajeRegistrar.text = "<color=red>Las claves no coinciden.";
			}
		} else {
			txtMensajeRegistrar.text = "Complete todos los campos";
		}
	}

	/// <summary>
	/// Este puede ser considerado un método especial, se encarga de esperar la respuesta del servicio
	/// web, ya sea si devuelve algun error o si se ejecuto de manera satisfactoria.
	/// </summary>
	/// <param name="www">
	/// Este parametro es donde se envio la informacion al servidor meditante una peticion 'Request'
	/// </param>
	/// <returns>
	/// devuelve la informacion recibida del servidor en formato Json o un mensaje de error(Si no
	/// se pudo conectar con el servicio).
	/// </returns>
	IEnumerator requestRegistro (WWW www) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("Faltan datos en formulario"))
				txtMensajeRegistrar.text = "Error:\n<color=red><i>Llene todos los campos</color>"; //error
			if (www.text.Contains ("La data no tiene el formato deseado"))
				txtMensajeRegistrar.text = "Error:\n<color=red><i>Ha ocurrido un error al enviar los datos al servidor, intente mas tarde.</color>"; //error
			if (www.text.Contains ("Correo ya registrado"))
				txtMensajeRegistrar.text = "Error:\n<color=red><i>Este correo esta actualmente en uso</color>"; //error
			if (www.text.Contains ("Operacion existosa")) {
				txtMensajeRegistrar.text = "<align=center><color=green>Se ha registrado correctamente"; //text of success
				//limpiar cajas
				limpiarRegistro ();
			}
		} else
			txtMensajeRegistrar.text = "<color=red>Error:\n</color>" + www.error; //error
	}

	/// <summary>
	/// Método que obtiene los parámetos de las cajas de texto para iniciar sesión, donde se
	/// verifica que no se envie informacion vacia o nula y se valida el correo electronico.
	/// Si la informacion para el inicio de sesion parece correcta se procede a enviarla como
	/// peticion 'Request' al servicio web.
	/// </summary>
	/// <remarks>
	/// Es necesario que este método sea publico para que pueda ser referenciado mediante el editor
	/// de Unity
	/// </remarks>
	public void btnIniciarSesion () {
		objPersona = new Persona (); //cada vez que se da al boton iniciar se crea un nuevo objeto persona
		objPersona.correo = inputCorreo.text.Trim (); //quitando los espacios en blanco (L & R)
		objPersona.clave = inputClave.text.Trim (); //quitando los espacios en blanco (L & R)
		//verfica que se haya escrito en las cajas de texto del inicio de sesión
		if (objPersona.correo == "" && objPersona.clave == "") {
			panelCargando.SetActive (true);
			btnAceptar.SetActive (true);
			txtMensaje.text = "Faltan datos por completar.";
			txtCargando.text = "";
			return;
		}
		//verifica si el correo esta en el formato correcto.
		if (validarCorreo (objPersona.correo)) {
			string json = JsonUtility.ToJson (objPersona);
			verificarSesion (json);
		} else {
			panelCargando.SetActive (true);
			btnAceptar.SetActive (true);
			txtMensaje.text = "El correo no tiene el formato deseado. \nEjemplo: a@b.c";
			txtCargando.text = "";
		}
	}

	/// <summary>
	/// Método reutilizable que recibe la informacion del inicio de sesión y la envia al servidor
	/// mediante una coroutina para que pueda ser procesada.
	/// </summary>
	/// <param name="json">
	/// Este parámetro recibe el correo y la clave en formato Json.
	/// </param>
	private void verificarSesion (string json) {
		panelCargando.SetActive (true); //activo la pantalla de carga
		banCarga = true;
		//string url = "localhost:80/Proyecto-Prog-Ava/Juego/public/index.php/login";//ruta local
		//string url = "https://eduquiza.azurewebsites.net/public/index.php/login"; //server azure
		string url = "https://eduquiz.000webhostapp.com/public/index.php/login"; //server free

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		txtMensaje.text = "Iniciando...";
		StartCoroutine (request (www));
	}

	/// <summary>
	/// Este método se encarga de esperar la respuesta del servicio web y mostrar un respectivo 
	/// mensaje al usuario, ya sea este un error o si la información es correcta.
	/// Si todo es correcto, la información (en formato Json) es guardada en un archivo local
	/// mediante 'PlayerPrefs' que sirve para almacenar datos en los Recursos de Android, luego de 
	/// esto se carga la escena del juego.
	/// </summary>
	/// <param name="www">
	/// Este parametro es donde se envio la informacion al servidor meditante una peticion 'Request'
	/// </param>
	/// <returns>
	/// devuelve la informacion recibida del servidor en formato Json o un mensaje de error(Si no
	/// se pudo conectar con el servicio).
	/// </returns>
	IEnumerator request (WWW www) {
		yield return www;

		btnAceptar.SetActive (true);
		if (string.IsNullOrEmpty (www.error)) {
			if (www.text.Contains ("Incompatibilidad de datos"))
				txtMensaje.text = "<color=red>Error:\n</color><i>Correo o contraseña esta incorrecta.</color>"; //error
			if (www.text.Contains ("No se ha encontrado ningun dato"))
				txtMensaje.text = "<color=red>Error:\n</color><i>Correo o contraseña esta incorrecta.</color>"; //error
			if (www.text.Contains ("Faltan datos"))
				txtMensaje.text = "<color=red>Error:\n</color><i>Se necesita un correo y una clave válidas.</color>"; //error
			if (www.text.Contains ("La data no tiene el formato deseado"))
				txtMensaje.text = "<color=red>Error:\n</color><i>Ha ocurrido un error al enviar los datos al servidor, intente mas tarde.</color>"; //error
			if (www.text.Contains ("Operacion existosa")) {
				btnAceptar.SetActive (false);
				txtMensaje.text = "<align=center><color=green>Correcto"; //text of success
				objPersona = JsonUtility.FromJson<Persona> (www.text);
				PlayerPrefs.SetString ("InfoJugador", www.text);
				if (inputClave.text != "" && (PlayerPrefs.GetString ("InfoClave", "") != inputClave.text)) {
					PlayerPrefs.SetString ("InfoClave", inputClave.text);
				}
				SceneManager.LoadScene ("game");
			}
		} else
			txtMensaje.text = "<color=red>Error:\n</color>" + www.error; //error

		StopCoroutine ("tiempoCarga");
		banCarga = false;
		txtCargando.text = "";
		inputCorreo.text = objPersona.correo;
		inputClave.text = "";
	}

	/// <summary>
	/// Método que verifica si se puede activar o desactivar la pantalla de 'carga' atraves de
	/// una bandera.
	/// </summary>
	private void pantallaCargando () {
		if (banCarga) {
			banCarga = false;
			StartCoroutine ("tiempoCarga");
		}
	}

	/// <summary>
	/// Muestra un mensaje en la pantalla de 'carga' y es desactivado automaticamente 
	/// en el momento en que el servidor de una respuesta y/o se cargue otra escena.
	/// </summary>
	/// <returns>
	/// Hace 'dormir' la ejecucion n segundos
	/// </returns>
	IEnumerator tiempoCarga () {
		yield return new WaitForSeconds (0.2f);
		switch (conCargaTexto) {
			case 1:
				txtCargando.text = "";
				break;
			case 2:
				txtCargando.text = ".";
				break;
			case 3:
				txtCargando.text = "..";
				break;
			case 4:
				txtCargando.text = "...";
				conCargaTexto = 0;
				break;
		}
		conCargaTexto++; //incremento el contador
		banCarga = true;
	}

	/// <summary>
	/// Este metodo es ejecutado cuando se toca el boton de 'Aceptar' cuando ocurre un error en la
	/// autenticacion o existen datos incorrecto, desactiva el panel de 'carga' y al mismo boton.
	/// </summary>
	/// <remarks>
	/// Es necesario que este método sea publico para que pueda ser referenciado mediante el editor
	/// de Unity
	/// </remarks>
	public void btnVolverA () {
		panelCargando.SetActive (false);
		btnAceptar.SetActive (false);
	}

	/// <summary>
	/// Este método simplemente se encarga de limpiar cada una de las cajas de texto que se encuentran
	/// en el panel de 'registrar usuario'.
	/// </summary>
	private void limpiarRegistro () {
		inputRegNombre.text = "";
		inputRegCorreo.text = "";
		inputRegClave.text = "";
		inputRegReClave.text = "";
	}

	/// <summary>
	/// Este método es reutilizable y se encarga de verificar que el correo ingresado por una
	/// caja de texto este en el formato de e-mail correspondiente.
	/// </summary>
	/// <param name="correo">
	/// Este parametro recibe el correo escrito y pasa a ser validado para ver si cumple el
	/// formato e-mail.
	/// </param>
	/// <returns>
	/// Devuelve true -> si el correo escrito tiene el formato deseado.
	/// Devuelve false -> si el correo escrito no cumple el formato deseado.
	/// </returns>
	private bool validarCorreo (string correo) {
		string expresion;
		expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
		if (Regex.IsMatch (correo, expresion)) {
			if (Regex.Replace (correo, expresion, string.Empty).Length == 0) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}
}