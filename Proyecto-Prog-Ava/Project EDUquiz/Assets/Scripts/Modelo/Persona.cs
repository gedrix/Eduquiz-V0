using System;
using UnityEngine;

[Serializable]
public class Persona
{
	public string nombre;
	public string correo;
	public string clave;
	public int rol = 1;// 0 = Admin | 1 = usuario comun
	public string imagen;
	public string external_id;
	public string token;
	//info del juego
	public int nivel = 1;
	public int experiencia = 0;
	public int clasificacion = 0;
	public int puntaje = 0;
}
