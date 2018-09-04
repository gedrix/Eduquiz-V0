using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase que permite conectar la vista de 'Logros' y los componentes que se encuentran en
/// el editor Unity.
/// </summary>
public class LogrosController : MonoBehaviour {
	public Canvas ui_logros; //obtiene la interfaz de 'logros'.
	public Canvas ui_main; //obtiene la interfaz de la pantalla 'principal'.
	public GameObject itemLogro; //prefab del objeto -> itemLogro para generar clones del mismo
	public Transform trnsItemLogro; //pivote que me sirve de referencia para la instancia de objetos
	public static int nuevoLogro = 0; //contador de cada vez que se desbloquea un logro.
	private Logros logros; //modelo auxiliar para almacenar los logros obtenidos en el servidor.
	Logros logrosDesbloqueados; //auxiliar para almacenar los logros desbloqueados por el jugador actual.
	ControlItemLogro controlItem; //para editar los parametros de cada item instanciado
	List<GameObject> listaItems = new List<GameObject> (); //almacena cada uno de los items

	/// <summary>
	/// Este método permite obtener una lista de todos los logros encontrados en la base de datos.
	/// </summary>
	/// <param name="token">sirve para autentificar la peticion al servidor</param>
	public void obtenerListaLogros (string token, string external) {
		string json = "{}";
		string url = "https://eduquiz.000webhostapp.com/public/index.php/logros/listarLogro"; //server free

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");
		headers.Add ("token", token);

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestListar (www, token, external));
	}

	/// <summary>
	/// Este método se encarga de que una vez obtenida la respuesta del servidor y este correctamente
	/// listar todos los logros en <GameObjetcs> y para que puedan ser visualizados en la vista
	/// 'logros', ademas estos objetos pasan a ser guardados en una lista para que puedan ser
	/// manipulados y/o destruidos
	/// </summary>
	/// <param name="www">Parametro que sirve para esperar la respuesta del servicio web</param>
	/// <returns>Espera la respuesta del servidor para listar los logros</returns>
	IEnumerator requestListar (WWW www, string token, string external) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			logros = JsonUtility.FromJson<Logros> (www.text);
			limpiarListaItems ();
			for (int i = 0; i < logros.ids.Length; i++) {
				GameObject obj = (GameObject) Instantiate (itemLogro);
				listaItems.Add (obj);
				obj.transform.parent = trnsItemLogro.parent;
				obj.transform.localScale = new Vector3 (1f, 1f, 1f);
				//obtengo el componente para editar la info de cada item
				controlItem = obj.GetComponent<ControlItemLogro> ();
				controlItem.editarInfoItem (logros.ids[i], logros.nombres[i],
					logros.descripciones[i], logros.imagenes[i]);
			}
			//una vez listados todos los logros se busca los desbloqueados por el jugador actual
			obtenerLogrosDesbloqueados (token, external);
		} else
			print ("Error al listar logros. " + www.error);
	}

	/// <summary>
	/// Este método se encarga de que una vez obtenida la respuesta del servidor y este correctamente
	/// listar todos los logros que hayan sido desbloqueado por el jugador actual, esto para realizar
	/// una comparacion de la lista de todos los logros y que se pueda visualizar los logros alcanzados
	/// en la pantalla de 'logros'.
	/// </summary>
	/// <param name="external">parametro para realizar la busqueda de los logros del jugador
	/// mediante el external_id que sirve como una llave publica.</param>
	/// <param name="token">parametro que sirve de autentificacion para recibir informacion
	/// del servicio web.</param>
	public void obtenerLogrosDesbloqueados (string token, string external) {
		string json = "{}";
		string url = "https://eduquiz.000webhostapp.com/public/index.php/usuario/listarLogro/" +
			external; //server free

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");
		headers.Add ("token", token);

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestDesbloqueados (www));
	}

	/// <summary>
	/// Este método se encarga de que una vez obtenida la respuesta del servidor y este correctamente
	/// obtener una lista de los logros debloqueados por el jugador que este realizando la peticion
	/// y de esta forma aparezcan en la interfaz de 'logros' como ya desbloqueados.
	/// </summary>
	/// <param name="www">Parametro que sirve para esperar la respuesta del servicio web</param>
	/// <returns>Espera la respuesta del servidor para listar los logros desbloqueados</returns>
	IEnumerator requestDesbloqueados (WWW www) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			logrosDesbloqueados = JsonUtility.FromJson<Logros> (www.text);
			for (int i = 0; i < logros.nombres.Length; i++) {
				// de esta manera evito una complejidad O(n)^2 y solo es O(n)log n 
				if (listaItems[i].GetComponent<ControlItemLogro> ().estado == "Bloqueado") {
					for (int j = 0; j < logrosDesbloqueados.nombres.Length; j++) {
						if (logrosDesbloqueados.nombres[j] == logros.nombres[i]) {
							listaItems[i].GetComponent<ControlItemLogro> ().desbloquearLogro ();
							break;
						}
					}
				}
			}
		} else
			print ("Error al listar logros desbloqueados. " + www.error);
	}

	/// <summary>
	/// Este método se encarga de recorrer la lista de objetos donde se almacenaron cada uno de los
	/// items y destruir los objetos, basicamente para que se vuelvan a generar una lista actualizada.
	/// Una vez destruidos todos los objetos se limpia la lista.
	/// </summary>
	private void limpiarListaItems () {
		foreach (GameObject item in listaItems) {
			Destroy (item);
		}
		listaItems.Clear ();
	}

	/// <summary>
	/// Este método es ejecutado mediante un boton, para volver a la pantalla 'principal' del juego.
	/// </summary>
	/// <remarks>
	/// Es necesario que este método sea publico para que pueda ser referenciado mediante el editor
	/// de Unity
	/// </remarks>
	public void btnRegresarMenu () {
		ui_logros.enabled = false;
		ui_main.enabled = true;
	}

	/// <summary>
	/// Este metodo esta encargado de controlar lo logros generales cada uno con diferentes
	/// condiciones es por ello que se decidio separar a los 'Logros de nivel' en un metodo diferente
	/// Este metodo es llamado cada vez que se finaliza una partida para verificar si algun logro
	/// cumplio la condicion requerida y se procede a 'desbloquearlo' mediante el metodo registrarLogro().
	/// </summary>
	/// <param name="external">necesario para realizar la busqueda del jugador actual</param>
	/// <param name="token">necesario para autentificar la peticion al servidor</param>
	public void controlLogros (string external, string token) {
		//ID = 1 - Juega 3 partidas
		if (GameController.conJugadas >= 3) {
			for (int i = 0; i < logros.nombres.Length; i++) {
				if (logros.nombres[i] == "Principiante") {
					if (listaItems[i].GetComponent<ControlItemLogro> ().estado == "Bloqueado") {
						registrarLogro (external, token, logros.ids[i], i);
						break;
					}
					break;
				}
			}
		}
		//Logros de N veces acertadas
		if (GameController.numAcertadas >= 4) { //4 preguntas acertas
			for (int i = 0; i < logros.nombres.Length; i++) {
				if (logros.nombres[i] == "Curioso(a)") {
					if (listaItems[i].GetComponent<ControlItemLogro> ().estado == "Bloqueado") {
						registrarLogro (external, token, logros.ids[i], i);
					}
				}
				if (GameController.numAcertadas >= 7) { //7 preguntas acertas
					if (logros.nombres[i] == "Suertudo(a)") {
						if (listaItems[i].GetComponent<ControlItemLogro> ().estado == "Bloqueado") {
							registrarLogro (external, token, logros.ids[i], i);
						}
					}
					if (GameController.numAcertadas >= 10 && logros.nombres[i] == "Sabelotodo") { //10 preguntas acertas
						if (listaItems[i].GetComponent<ControlItemLogro> ().estado == "Bloqueado") {
							registrarLogro (external, token, logros.ids[i], i);
							break;
						}
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Este metodo se encarga de controlar si se ha desbloqueado algun logro de 'nivel'
	/// este metodo es llamada siempre que se termine de completar una partida, porque es cuando la 
	/// experiencia del jugador sube y por ende el nivel.
	/// </summary>
	/// <param name="nivel">necesario para conocer el nivel actual del jugador</param>
	/// <param name="external">necesario para realizar la busqueda del jugador actual</param>
	/// <param name="token">necesario para autentificar la peticion al servidor</param>
	public void controlLogrosDeNivel (int nivel, string external, string token) {
		if (nivel >= 5) {
			for (int i = 0; i < logros.nombres.Length; i++) {
				if (nivel >= 5 && logros.nombres[i] == "Aprendiz") {
					if (listaItems[i].GetComponent<ControlItemLogro> ().estado == "Bloqueado") {
						registrarLogro (external, token, logros.ids[i], i);
					}
				}
				if (nivel >= 10 && logros.nombres[i] == "Estudiante") {
					if (listaItems[i].GetComponent<ControlItemLogro> ().estado == "Bloqueado") {
						registrarLogro (external, token, logros.ids[i], i);
					}
				}
				if (nivel >= 15 && logros.nombres[i] == "Conocedor") {
					if (listaItems[i].GetComponent<ControlItemLogro> ().estado == "Bloqueado") {
						registrarLogro (external, token, logros.ids[i], i);
					}
				}
				if (nivel >= 20 && logros.nombres[i] == "Investigador") {
					if (listaItems[i].GetComponent<ControlItemLogro> ().estado == "Bloqueado") {
						registrarLogro (external, token, logros.ids[i], i);
					}
				}
				if (nivel >= 25 && logros.nombres[i] == "Maestro") {
					if (listaItems[i].GetComponent<ControlItemLogro> ().estado == "Bloqueado") {
						registrarLogro (external, token, logros.ids[i], i);
						break;
					}
					break;
				}
			}
		}
	}

	/// <summary>
	/// Este metodo reutilizable para ser llamado cada vez que se cumpla una condicion de algun
	/// logro 'Desbloqueo de logros' y para ello es necesario informacion del jugador y del logro
	/// que se llego a desbloquear.
	/// </summary>
	/// <param name="external">necesario para realizar la busqueda del jugador actual</param>
	/// <param name="token">necesario para autentificar la peticion al servidor</param>
	/// <param name="idLogro">el identificador del logro que se desbloqueo en la base de datos</param>
	/// <param name="posLogro">posicion del logro en la vista 'Logros' (no es lo mismo que el ID)</param>
	public void registrarLogro (string external, string token, int idLogro, int posLogro) {
		string json = "{\"external_id\":\"" + external + "\", \"id_logro\":\"" + idLogro + "\"}";
		string url = "https://eduquiz.000webhostapp.com/public/index.php/usuario/registrarLogro"; //server free

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");
		headers.Add ("token", token);

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestRegistroLogro (www, posLogro));
	}

	/// <summary>
	/// .!--.
	/// </summary>
	/// <param name="www">Parametro que sirve para esperar la respuesta del servicio web</param>
	/// <returns>Espera la respuesta del servidor para listar los logros desbloqueados</returns>
	IEnumerator requestRegistroLogro (WWW www, int posLogro) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			nuevoLogro++;
			listaItems[posLogro].GetComponent<ControlItemLogro> ().desbloquearLogro ();
		} else
			print ("Error al registrar el logro" + www.error);
	}
}