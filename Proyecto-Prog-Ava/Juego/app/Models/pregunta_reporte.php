<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class pregunta_reporte extends Model{
    /*referencia a la tabla de la base de datos*/
    protected $table = 'pregunta_reporte';
    //para saber si en la tabla usamos created_at y updated_at
    public $timestamps = true;
    //lista blancas campos publicos
    protected $fillable = ['motivo', 'created_at', 'updated_at', 'id_persona', 'id_pregunta', 'external_id_rep'];
    //lista negra campos que no quieren que se encuentren facilmente
    protected $guarded = ['id'];
    
    //Relacion PERNECE A
    public function Persona(){
        return $this->belongsTo('App\Models\persona', 'id_persona');
    }

    //Relacion PERNECE A
    public function Pregunta(){
        return $this->belongsTo('App\Models\pregunta', 'id_pregunta');
    }
}