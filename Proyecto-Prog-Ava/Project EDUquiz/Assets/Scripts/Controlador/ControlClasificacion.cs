using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que permite conectar la vista de 'Clasificacion' y los componentes que se encuentran en
/// el editor Unity.
/// </summary>
public class ControlClasificacion : MonoBehaviour {
	public GameObject itemClasificacion; //prefab del objeto -> itemClasificacion para generar clones del mismo
	public Transform trnsItemClas; //pivote que me sirve de referencia para la instancia de objetos
	Personas personas;
	//referencia a otros scripts con Monobehaviour
	ControlItemClasificacion controlItem; //para editar los parametros de cada item instanciado
	List<GameObject> listaItems = new List<GameObject> (); //almacena cada uno de los items

	/// <summary>
	/// Este método permite obtener una lista de N jugadores que se encuentren en la clasificacion
	/// </summary>
	/// <param name="token">sirve para autentificar la peticion al servidor</param>
	/// <param name="jugador">nombre del jugador que es comparado de los otros para que sea
	/// diferenciado del resto de jugadores en la lista clasificatoria</param>
	public void obtenerListaClasificacion (string token, string jugador) {
		string json = "{}";
		//string url = "https://eduquiza.azurewebsites.net/public/index.php/usuario/listaClasificacionJuego"; //server azure
		string url = "https://eduquiz.000webhostapp.com/public/index.php/usuario/listaClasificacionJuego"; //server free

		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json; charset=utf-8");
		headers.Add ("token", token);

		byte[] pData = System.Text.Encoding.UTF8.GetBytes (json.ToCharArray ());

		WWW www = new WWW (url, pData, headers);
		StartCoroutine (requestLista (www, jugador));
	}

	/// <summary>
	/// Este método se encarga de que una vez obtenida la respuesta del servidor y este correctamente
	/// listar a los jugadores en <GameObjetcs> y para que puedan ser visualizados en la vista
	/// 'clasificacion', ademas estos objetos pasan a ser guardados en una lista para que puedan ser
	/// manipulados y/o destruidos
	/// </summary>
	/// <param name="www">Parametro que sirve para esperar la respuesta del servicio web</param>
	/// <param name="correo">correo del jugador que es comparado de los otros para que sea
	/// diferenciado del resto de jugadores en la lista clasificatoria</param>
	/// <returns>Espera la respuesta del servidor para listar los jugadores</returns>
	IEnumerator requestLista (WWW www, string correo) {
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			personas = JsonUtility.FromJson<Personas> (www.text);
			limpiarListaItems ();
			for (int i = 0; i < personas.nombres.Length; i++) {
				GameObject obj = (GameObject) Instantiate (itemClasificacion);
				listaItems.Add (obj);
				obj.transform.parent = trnsItemClas.parent;
				obj.transform.localScale = new Vector3 (1f, 1f, 1f);
				//obtengo el componente para editar la info de cada item
				controlItem = obj.GetComponent<ControlItemClasificacion> ();
				controlItem.editarInfoItem (personas.nombres[i], personas.puntajes[i], i + 1,
					personas.imagenes[i]);
				if (correo == personas.correos[i])
					controlItem.colorItemJugador ();
			}
		} else
			print ("Error al listar clasificacion. " + www.error);
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
}