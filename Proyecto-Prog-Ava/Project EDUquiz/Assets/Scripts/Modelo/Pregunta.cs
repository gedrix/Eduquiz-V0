using System;
using UnityEngine;

/// <summary>
/// Esta clase es utilizada como un objeto para almacenar la informacion de la pregunta actual que
/// es mostrada durante el juego.
/// </summary>
/// <remarks>
/// NOTA: los 'modelos' necesariamente deben incluir al inicio [Serializable], lo cual serializa
/// cada uno de los parametros y permite que Unity identifique a esta clase como un objeto 'modelo'. 
/// </remarks>
[Serializable]
public class Pregunta {
	public int id; //es necesario conocer el id de la pregunta para evitar la redundancia de la misma en una partida.
	public string pregunta; //nombre o titulo de la pregunta
	public string dificultad; //dificulta en la que se encuentra la pregunta
	public string creadaPor; //nombre del usuario quien la creó
	public int cantidad; //necesario para recibir N preguntas aleatorias
	public string[] categoria; //arreglo, porque puede tener mas de una categoria
	public string[] opcion = new string[4]; //las cuatro opciones alternativas a la pregunta
	//public string[] opcionEstado = new string[4];//tambien guardo el estado correspondiente de cada opcion
}