using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SesionController : MonoBehaviour {
	public TMP_InputField inputCorreo; //otiene lo escrito en la caja de correo
	public TMP_InputField inputClave; //obtiene lo escrito en la caja de contraseña
	public TextMeshProUGUI txtMensaje; //muestra los mensajes de error
	public TextMeshProUGUI txtCargando;

	//animacion del registro
	public Animator animReg;
	private Persona objPersona;
	//datos de registro
	public TMP_InputField inputRegNombre;
	public TMP_InputField inputRegCorreo;
	public TMP_InputField inputRegClave;
	public TMP_InputField inputRegReClave;
	public TextMeshProUGUI txtMensajeRegistrar; //muestra los mensajes de error
	public GameObject panelCargando;
	public GameObject btnAceptar;
	private bool banCarga = false;
	private int conCargaTexto = 0;
	private void Awake () {
		panelCargando.SetActive (false);
	}
	public void Start () {
		btnAceptar.SetActive (false);
		//PlayerPrefs.DeleteAll (); //USAR CON CUIDADO... <comentar esto al compilar el juego> NO TE OLVIDES
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
	public void btnRegistrarse () {
		animReg.SetBool ("banRegistrar", true);
	}

	public void btnRegresar () {
		animReg.SetBool ("banRegistrar", false);
	}

	public void btnCrearCuenta () {
		if (inputRegNombre.text != "" && inputRegCorreo.text != "" && inputRegClave.text != "" &&
			inputRegReClave.text != "") {
			if (inputRegClave.text == inputRegReClave.text) { //si las claves coinciden
				objPersona = new Persona (); //cada vez que se da al boton iniciar se crea un nuevo objeto persona
				objPersona.nombre = inputRegNombre.text.Trim (); //quitando los espacios en blanco (L & R)
				objPersona.correo = inputRegCorreo.text.Trim (); //quitando los espacios en blanco (L & R)
				objPersona.clave = inputRegClave.text.Trim (); //quitando los espacios en blanco (L & R)

				string json = JsonUtility.ToJson (objPersona);
				print (json);
				string url = "https://eduquiza.azurewebsites.net/public/index.php/usuario/registro";

				Dictionary<string, string> headers = new Dictionary<string, string> ();
				headers.Add ("Content-Type", "application/json; charset=utf-8");
				//maldito UTF_8!!!!
				byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

				WWW www = new WWW (url, pData, headers);
				txtMensajeRegistrar.text = "Espere...";
				StartCoroutine (requestRegistro (www));
			} else {
				inputRegClave.text = "";
				inputRegReClave.text = "";
				print ("Las claves no coinciden.");
			}
		} else {
			print ("Llenar todos los campos");
		}
	}
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

	public void btnIniciarSesion () {
		objPersona = new Persona (); //cada vez que se da al boton iniciar se crea un nuevo objeto persona
		objPersona.correo = inputCorreo.text.Trim (); //quitando los espacios en blanco (L & R)
		objPersona.clave = inputClave.text.Trim (); //quitando los espacios en blanco (L & R)

		string json = JsonUtility.ToJson (objPersona);
		verificarSesion (json);
	}
	private void verificarSesion (string json) {
		panelCargando.SetActive (true); //activo la pantalla de carga
		banCarga = true;
		//string url = "localhost:80/Proyecto-Prog-Ava/Juego/public/index.php/login";//ruta local
		string url = "https://eduquiza.azurewebsites.net/public/index.php/login"; //ruta servicio web

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");

		//byte[] pData = System.Text.Encoding.UTF8.GetBytes (json);
		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		txtMensaje.text = "Iniciando...";
		StartCoroutine (request (www));
	}
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
	private void pantallaCargando () {
		if (banCarga) {
			banCarga = false;
			StartCoroutine ("tiempoCarga");
		}
	}
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
	public void btnVolverA () {
		panelCargando.SetActive (false);
		btnAceptar.SetActive (false);
	}
	private void limpiarRegistro () {
		inputRegNombre.text = "";
		inputRegCorreo.text = "";
		inputRegClave.text = "";
		inputRegReClave.text = "";
	}
}