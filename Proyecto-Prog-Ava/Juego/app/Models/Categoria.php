<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
class categoria extends Model{
    /*referencia a la tabla de la base de datos*/
    protected $table = 'categoria';
    //para saber si en la tabla usamos created_at y updated_at
    public $timestamps = false;
    //lista blancas campos publicos
    protected $fillable = ['nombre', 'external_id', 'imagen'];
    //lista negra campos que no quieren que se encuentren facilmente
    protected $guarded = ['id'];
    
    //Relacion UNO a MUCHOS
    public function Preg_Cate(){
        return $this->hasMany('App\Models\preg_cate', 'id_categoria');
    }
}