using System;
using UnityEngine;

/// <summary>
/// Esta clase es utilizada como un objeto para almacenar la informacion del usuario actual,
/// para validar la sesion, datos del juego como experiencia y nivel y la autentificacion para
/// el servicio web -> token.
/// </summary>
/// <remarks>
/// NOTA: los 'modelos' necesariamente deben incluir al inicio [Serializable], lo cual serializa
/// cada uno de los parametros y permite que Unity identifique a esta clase como un objeto 'modelo'. 
/// </remarks>
[Serializable]
public class Persona
{
	public string nombre; //nombre del jugador
	public string correo; //correo del jugador
	public string clave; //contraseña del jugador
	public int rol = 1; //0 = Admin | 1 = usuario comun
	public string imagen; //nombre de la imagen que esta usando el jugador
	public string external_id; //identificador publico para reconocer la jugador
	public string token; //identificador encriptado para autentificar al jugador
	//info del juego
	public int nivel = 1; //nivel actual que tenga el jugador, por defecto el nivel 1
	public int experiencia = 0; //experiencia actual del jugador, por defecto 0
	public int puntaje = 0; //puntaje para la clasificacion del jugador, por defecto 0
}