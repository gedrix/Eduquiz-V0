<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class preg_cate extends Model{
    /*referencia a la tabla de la base de datos*/
    protected $table = 'preg_cate';
    //para saber si en la tabla usamos created_at y updated_at
    public $timestamps = false;
    //lista blancas campos publicos
    protected $fillable = ['id_pregunta', 'id_categoria'];
    
    //Relacion MUCHOS a UNO (PERNECE A)
    public function Pregunta(){
        return $this->belongsTo('App\Models\pregunta', 'id_pregunta');
    }
    //Relacion MUCHOS a UNO (PERNECE A)
    public function Categoria(){
        return $this->belongsTo('App\Models\categoria', 'id_categoria');
    }
}