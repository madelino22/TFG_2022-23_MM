import pandas as pd
import numpy as np
import scipy.io as scp
import scipy.optimize as scy
import matplotlib.pyplot as plt
import copy
import math
import seaborn as sns
import json
import csv
from unidecode import unidecode # acentos

def CorrectString(palabra):
    "Quita acentos, mayusculas, etc"
    palabra = palabra.lower() # quitar mayusculas
    palabra = palabra.replace("á", "a")
    palabra = palabra.replace("é", "e")
    palabra = palabra.replace("í", "i")
    palabra = palabra.replace("ó", "o")
    palabra = palabra.replace("ú", "u")
    palabra = palabra.replace("ñ", "n")
    palabra = palabra.replace("ç", "c")
    palabra = palabra.replace(" ", "")
    palabra = palabra.strip() # quitar espacios
    return palabra

# PARTE 1:==============================================================================
def Firebase(dic, data, user_names):
    for i in data['User']:
        # NO USAMOS "Este es mi usuario" o Jugadores con 0 partidas jugadas
        if i != "Este es mi usuario" and data['User'][i]['gamesPlayed'] != 0:
            # Recorremos atributos del user
            for j in data['User'][i]:
                # Guardamos los atributos que nos interesan
                if(j != "userName" and j != "zzlastGameSaved" and j != "email"):
                    dic[j] = np.concatenate((dic[j], [data['User'][i][j]]))
                # Guardamos los nombres en un array extra
                elif (j == "userName"):
                    new_name = data['User'][i][j]
                    new_name = CorrectString(new_name)
                    user_names = np.concatenate((user_names, [new_name]))
    return dic, user_names

# PARTE 2:==============================================================================
def BigFive(dic, user_names, file_name, variableNames): #'a.csv'
    "PROCESAMOS EL CSV DE BIG FIVE"

    variableNames = np.concatenate((variableNames, ["Extraversion", "Amabilidad", "Responsabilidad", "Apertura", "Neuroticismo"]))
    dic["Extraversion"] = []
    dic["Amabilidad"] = []
    dic["Responsabilidad"] = []
    dic["Apertura"] = []
    dic["Neuroticismo"] = []

    with open(file_name, newline='', encoding='utf-8') as archivo_csv: #sin el utf-8 petaba
        lector_csv = csv.reader(archivo_csv)
        next(lector_csv)

        # RECORREMOS LOS USARIOS DE FIREBASE
        for user_name in user_names:
            encontrado = False

            # BUSCAR FILA EN CSV
            for fila in lector_csv:
                nombre = fila[1]
                nombre = CorrectString(nombre)

                if nombre == user_name:
                    encontrado = True
                    ex = fila[5]
                    ex2=fila[7]
                    ex3=fila[9]
                    ex4=fila[11]
                    ex5=fila[13]
                    #print(nombre, ex, ex2, ex3, ex4, ex5)
                    res=int(ex)
                    res2=int(ex2)
                    res3=int(ex3)
                    res4=int(ex4)
                    res5=int(ex5)

                    extraversion = res/12*100
                    amabilidad = (res2/12)*100
                    responsabilidad = (res3/12) * 100
                    apertura = (res4/12) * 100
                    neuroticismo = (res5/12)*100

                    dic["Extraversion"] = np.concatenate((dic["Extraversion"], [extraversion]))
                    dic["Amabilidad"] = np.concatenate((dic["Amabilidad"], [amabilidad]))
                    dic["Responsabilidad"] =  np.concatenate((dic["Responsabilidad"], [responsabilidad]))
                    dic["Apertura"] = np.concatenate((dic["Apertura"], [apertura]))
                    dic["Neuroticismo"] = np.concatenate((dic["Neuroticismo"], [neuroticismo]))
                    archivo_csv.seek(0) # Volvemos al principio del archivo
                    break

            if encontrado == False:
                print("ERROR: Nombre " + user_name + " no existe en Big Five.")

            archivo_csv.seek(0) # Volvemos al principio del archivo
    return dic, variableNames

# PARTE 3:==============================================================================
def Satisfaccion(dic, user_names, file_name, variableNames):
    "PROCESAMOS EL CSV DE BIG FIVE"

    variableNames = np.concatenate((variableNames, ["1ª vez jugando", "Familiaridad", "Objetivo juego", "Disfrute", "Mejor q team", "Mejor q rivals"]))
    dic["1ª vez jugando"] = []
    dic["Familiaridad"] = []
    dic["Objetivo juego"] = []
    dic["Disfrute"] = []
    dic["Mejor q team"] = []
    dic["Mejor q rivals"] = []

    with open(file_name, newline='', encoding='utf-8') as archivo_csv: #sin el utf-8 petaba
        lector_csv = csv.reader(archivo_csv)
        next(lector_csv)

        # RECORREMOS LOS USARIOS DE FIREBASE
        for user_name in user_names:
            encontrado = False

            # BUSCAR FILA EN CSV
            for fila in lector_csv:
                nombre = fila[2]
                nombre = CorrectString(nombre)

                if nombre == user_name:
                    encontrado = True
                    first_time = fila[5] #Sí, No
                    familiar_control=fila[6] # Sí, No
                    understands_goal=fila[7] # Sí, No
                    fun=fila[8] # 1, 2, 3, 4, 5
                    best_in_team=fila[9] # 1, 2, 3, 4, 5
                    better_team=fila[10] # 1, 2, 3, 4, 5

                    res0 = 0
                    res1 = 0
                    res2 = 0
                    res3 = int(fun)
                    res4 = int(best_in_team)
                    res5 = int(better_team)

                    if first_time == 'Sí':
                        res0 = 1
                    if familiar_control == 'Sí':
                        res1 = 1
                    if understands_goal == 'Sí':
                        res2 = 1

                    dic["1ª vez jugando"] = np.concatenate((dic["1ª vez jugando"], [res0]))
                    dic["Familiaridad"] = np.concatenate((dic["Familiaridad"], [res1]))
                    dic["Objetivo juego"] = np.concatenate((dic["Objetivo juego"], [res2]))
                    dic["Disfrute"] = np.concatenate((dic["Disfrute"], [res3]))
                    dic["Mejor q team"] = np.concatenate((dic["Mejor q team"], [res4]))
                    dic["Mejor q rivals"] = np.concatenate((dic["Mejor q rivals"], [res5]))

                    archivo_csv.seek(0) # Volvemos al principio del archivo
                    break

            if encontrado == False:
                print("ERROR: Nombre " + user_name + " no existe en Satisfacción.")

            archivo_csv.seek(0) # Volvemos al principio del archivo
    return dic, variableNames

# MAIN==================================================================================
def main():
    firebase_file = 'Firebase/PruebasMM3.json'
    big_five_file = 'BigFivePrueba1.csv'
    satisfaccion_file = 'Satisfaccion.csv'
    #-------------RECOGIDA DE DATOS----------------
    # df = {
    # 'x': [45, 37, 42, 35, 39],
    # 'y': [38, 31, 26, 28, 33],
    # 'z': [10, 15, 17, 21, 12]
    # }
 
    with open(firebase_file, encoding="utf-8") as user_files:
        file_contents = user_files.read()

    data = json.loads(file_contents)
    
    dic = { } # cada fila es un usuario, cada columna una variable dic[usario] = [deaths, kills, ...]
    user_names = []

    #INICIALIZACION DATA: PUEDE DAR ERROR SI NO ESTE EL USUARIO, CAMBIAR POR OTRO QUE ESTE
    variableNames = []
    for j in data['User']["Este es mi usuario"]: 
        if(j != "userName" and j != "zzlastGameSaved" and j != "email"):
            dic[j] = []
            variableNames = np.concatenate((variableNames, [j]))

    # PARTE 1:==============================================================================
    #Pasar el JSON a un diccionario
    dic, user_names = Firebase(dic, data, user_names)
    
    # PARTE 2:==============================================================================
    # Añadir los datos de Big Five al dicionario
    dic, variableNames = BigFive(dic, user_names, big_five_file, variableNames)

    # PARTE 3:============================================================================== 
    # Añadir los datos de satisfaccion
    dic, variableNames = Satisfaccion(dic, user_names, satisfaccion_file, variableNames)

    # form dataframe
    df = pd.DataFrame(dic, columns=variableNames)

    #-------------MATRIZ DE CORRELACION----------------
    matrix = df.corr(method = 'pearson').round(2)  # The method of correlation
    
    #.............VISUALIZACION-----------------------
    sns.heatmap(matrix, annot=True, vmax=1, vmin=-1, center=0, cmap='vlag')
    plt.show()

main()