using System;
using UnityEngine;

[Serializable]
public class Pregunta {
	public int id;//es necesario conocer el id de la pregunta para evitar la redundancia de la misma en una partida.
	public string pregunta;//nombre o titulo de la pregunta
	public string dificultad;//la palabra mismo lo dice...
	public string creadaPor;//nombre del usuario quien la creó
	public int cantidad;//necesario para recibir N preguntas aleatorias
	public string[] categoria;//arreglo, porque puede tener mas de una categoria
	public string[] opcion = new string[4];//las cuatro opciones alternativas a la pregunta
	//public string[] opcionEstado = new string[4];//tambien guardo el estado correspondiente de cada opcion
}