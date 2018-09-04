using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase que es utilizada para instanciar objetos en Unity con los atributos y metodos definidos
/// </summary>
public class ControlItemClasificacion : MonoBehaviour {
	public TextMeshProUGUI txtNombreUsu; //muestra nombre del usuario
	public TextMeshProUGUI txtPuntaje; //para mostrar los puntos de clasificacion
	public TextMeshProUGUI txtPosicion; //muestra el numero de la posicion en la que se encuentra
	private ChangeImage img; //hace referencia a la clase hija de este objeto para obtener su imagen
	private Image colorItem; //hace referencia al componente Image para modificar sus parametros.

	/// <summary>
	/// Este metodo es el primero en iniciarse y se ejecuta solo una vez, necesario para referenciar
	/// componentes o definir el valor inicial de una variable, en este caso utilizado para hacer
	/// referencia a la clase <ChangeImage> del componente hijo y a la clase <Image>.
	/// </summary>
	void Awake () {
		img = GetComponentInChildren<ChangeImage> ();
		colorItem = GetComponent<Image> ();
	}

	/// <summary>
	/// Este método esta encargado de settear la informacion al momento de que un objeto sea 
	/// instanciado en Unity.
	/// </summary>
	/// <param name="nombre">Para mostrar el nombre del jugador</param>
	/// <param name="puntaje">Para mostrar el puntaje actual</param>
	/// <param name="pos">Muestra el numero en que se encuentra el jugador</param>
	/// <param name="imagen">Nombre de la imagen del jugador para que sea cargada</param>
	public void editarInfoItem (string nombre, int puntaje, int pos, string imagen) {
		txtNombreUsu.text = nombre;
		txtPuntaje.text = puntaje + "";
		txtPosicion.text = pos + "";
		img.SetImage (Resources.Load<Sprite> ("Images/Users/" + imagen));
	}

	/// <summary>
	/// Este metodo al ser llamado modifica el color del objeto en el que fue instanciado para
	/// distinguirse de los demas objetos.
	/// </summary>
	public void colorItemJugador () { //establesco un color diferente para el jugador 
		colorItem.color = new Color (0, 220, 0);
	}
}