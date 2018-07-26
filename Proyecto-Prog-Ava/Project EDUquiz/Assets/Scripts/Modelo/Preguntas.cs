using System;
using UnityEngine;

[Serializable]
public class Preguntas {
	public int[] ids; //es necesario conocer el id de la pregunta para evitar la redundancia de la misma en una partida.
	public string[] preguntas; //nombre o titulo de la pregunta
	public string[] dificultad; //la palabra mismo lo dice...
}