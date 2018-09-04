using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase que es utilizada para cambiar u obtener la imagen de un componente en el cual este 
/// asignado el script.
/// </summary>
public class ChangeImage : MonoBehaviour {
	private Image img; //para hacer referencia al componente <Image> de manera interna

	/// <summary>
	/// Este metodo es el primero en iniciarse y se ejecuta solo una vez, necesario para referenciar
	/// componentes o definir el valor inicial de una variable, en este caso es utilizado para hacer
	/// referencia al componente <Image> en el cual este asignado el script.
	/// </summary>
	void Awake () {
		img = GetComponent<Image> ();
	}

	/// <summary>
	/// Este método permite cambiar la imagen por una que sea receptada mediante el parametro newImg.
	/// </summary>
	/// <param name="newImg">Usado para cambiar la imagen actual del componente</param>
	public void SetImage (Sprite newImg) {
		img.sprite = newImg;
	}

	/// <summary>
	/// Este método permite obtener la imagen actual que contenga el componente en el que se 
	/// encuentre el script.
	/// </summary>
	/// <returns>Devuelve la imagen actual como objeto tipo '<Sprite>'. </returns>
	public Sprite GetImage () {
		return img.sprite;
	}
}