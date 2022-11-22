
import re
import random
import copy

class Partida:
    averageRatingA = 0
    averageRatingB = 0
    teamA =[]
    teamB = []
#--------------------------------------------------------------------------------------------
def read(name):
    "Lee un archivo de Jugadores [Name, Rating, K] y lo devuelve en una tupla"
    #LEER EN TXT
    lines =[]
    archivo = open(name)
    with archivo as f:
        lines = f.readlines()

    jugadores = []
    for line in lines:
        words = re.sub(r'\W+', ' ', line).split() 
        #for word in words:
         #   print(word, end=" ")
        jugadores.append(words)

    archivo.close()
    for j in jugadores:
        j[1] = str(10)
        print("Nombre: " + j[0] + ", Rating: " + j[1] + ", K: " + j[2])

    return jugadores

#---------------------------------------------------------------------------------------------
def generatePlayers(num):
    "Genera una tupla con num [Name, Rating, K]"
    jugadores = []
    for i in range(num):
        rating = random.randint(0, 3000)

        proporcion = 1 - (rating / 3000) # si es muy bueno = 0, malo = 1
        k = 10 + 30 * proporcion 
        k = int(k)
        if k >= 11 & k <= 39:
            k = k + (random.randint(-100, 100) / 100)

       # k = random.randint(10, 40)
        jugadores.append(["Player"+str(i), str(rating),str(k)])
    return jugadores

#---------------------------------------------------------------------------------------------
def write(name, jugadores):
    "Escribe en un txt una tupla de jugadores [Name, Rating, K]"
    archivo = open(name, "w")
    for j in jugadores:
        archivo.write(j[0] + " " + j[1] + " " + j[2] + "\n")

    archivo.close()


def mergeSortByRating(array):
    "Ordena una tupla de jugadores en funcion del rating"
    if len(array) > 1:
        mid = len(array) // 2 #quita los decimales
        left = array[:mid]
        right = array[mid:]

        mergeSortByRating(left)
        mergeSortByRating(right)

        i = 0
        j = 0
        k = 0

        while i < len(left) and j < len(right):
            if int(left[i][1]) < int(right[j][1]) or (int(left[i][1]) == int(right[j][1]) and float(left[i][2]) <= float(right[j][2])):
                array[k] = left[i]
                i += 1

            else:
                array[k] = right[j]
                j += 1
            k += 1

        while i < len(left):
            array[k] = left[i]
            i += 1
            k += 1

        while j < len(right):
            array[k] = right[j]
            j += 1
            k += 1

    return array

def mergeSortById(array):
    "Ordena una tupla de jugadores en funcion del Name"
    #Player1 > Player2 > Player3....
    if len(array) > 1:
        mid = len(array) // 2 #quita los decimales
        left = array[:mid]
        right = array[mid:]

        mergeSortById(left)
        mergeSortById(right)

        i = 0
        j = 0
        k = 0

        while i < len(left) and j < len(right):
            if str(left[i][0]) < str(right[j][0]):
                array[k] = left[i]
                i += 1

            else:
                array[k] = right[j]
                j += 1
            k += 1

        while i < len(left):
            array[k] = left[i]
            i += 1
            k += 1

        while j < len(right):
            array[k] = right[j]
            j += 1
            k += 1

    return array

def createTeams(jugadores):
    "Crea una partida de 6 jugadores. Recuerda que deben estar ordenados mediante el mergeSortByRating"
    # De momento los distribuye en base al Rating:
    # Si me llega ratings(6, 5, 4, 3, 2, 1) la distribucion sera ==> (6, 4, 2) & (5, 3, 1)
   
    l = len(jugadores)
    if(l != 6):
        print("Intento formar partida con " + str(l) + " miembros en vez de 6")
        return

    teamA = []
    teamB = []
    averageRatingA = 0
    averageRatingB = 0

    for j in range(l):
        if j % 2 == 0:
            averageRatingA += int(jugadores[j][1])
            teamA.append(jugadores[j])
        else:
            averageRatingB += int(jugadores[j][1])
            teamB.append(jugadores[j])

    averageRatingA /= 3
    averageRatingB /= 3

    # print("Team A: ")
    # print(teamA)
    # print("TeamB")
    # print(teamB)
    
    partida = Partida()
    partida.averageRatingA = averageRatingA
    partida.averageRatingB = averageRatingB
    partida.teamA = teamA
    partida.teamB = teamB

    return partida

#---------------------------------------------------------------------------------------
def createPartidas(jugadores, numPlayers, jugadoresPostMatch):
    "Crea x partidas con numPlayers disponibles. Los jugadores que sin grupo se guardan en jugadoresPostMatch"
    ini = 0
    numPlayersInMatch = 6
    fin = numPlayersInMatch
    partidas = []
    #FORMAMOS GRUPOS
    while fin <= numPlayers: 
        print()
        partida = createTeams(jugadores[ini:fin])
        partidas.append(partida)
        ini += numPlayersInMatch
        fin += numPlayersInMatch

    #GUARDAMOS LOS QUE NO TIENEN GRUPO PARA LUEGO
    while ini < numPlayers: 
       # print("JAJAJAJA "+str(ini))
        jugadoresPostMatch.append(jugadores[ini])
        ini += 1

    return partidas

#------------------------------------------------------------------------------------------
def determineWinner(partida, jugadores):
    "jugadores must have been organized through MergeSort. Only 6 members"
   
    # ESTIMACION
    eA = 1 / (1 + 10**((partida.averageRatingB - partida.averageRatingA) / 400))
    eB = 1 / (1 + 10**((partida.averageRatingA - partida.averageRatingB) / 400))

    # RESULTADOS PARTIDA
    sA = 0
    if partida.averageRatingA > partida.averageRatingB:
        sA = 1
    elif partida.averageRatingA == partida.averageRatingB:
        sA = 0.5
    else:
        sA = 0

    sB = 1 - sA

    #RECALCULAR RATING
    teamA = partida.teamA
    for j in teamA:
        #RA' = RA + K * (sA - eA) 
        j[1] = str(int(int(j[1]) + float(j[2]) * (sA - eA)))
        if int(j[1]) > 3000:
            j[1] = "3000"
        elif int(j[1]) < 0:
            j[1] = "0"
        jugadores.append(j)
    
    teamB = partida.teamB
    for j in teamB:
        #RA' = RA + K * (sA - eA) 
        j[1] = str(int(int(j[1]) + float(j[2]) * (sB - eB)))
        if int(j[1]) > 3000:
            j[1] = "3000"
        elif int(j[1]) < 0:
            j[1] = "0"
        jugadores.append(j)

    partida.teamA = teamA
    partida.teamB = teamB

    return jugadores

def compareResults(jugadoresBeforeMatch, jugadoresPostMatch, results):
    "Escribe en result.txt la diff de nivel de cada jugador. Recuerda ordenar a los jugadores con mergeSortById."

    if len(jugadoresBeforeMatch) != len(jugadoresPostMatch):
        print("Error en compareReults diff len")
        return
    
    archivo = open(results, "w") #permiso escritura

    l = len(jugadoresBeforeMatch)
    for i in range(l):
        if jugadoresBeforeMatch[i][0] != jugadoresPostMatch[i][0]:
            print("error en compareResults. Los arrays no estan ordenados " + jugadoresBeforeMatch[i][0] + " " +jugadoresPostMatch[i][0])
            return
        archivo.write("Player" + jugadoresBeforeMatch[i][0] + " " + str(int(jugadoresPostMatch[i][1]) - int(jugadoresBeforeMatch[i][1])) + " " + jugadoresBeforeMatch[i][2] + "\n")
    archivo.close()


def main():
    print("Hello World!")
    #LEER TXT----------------------------------
    numPlayers = 15
   
    #GENERAR STATS----------------------------
    jugadores = generatePlayers(numPlayers)
    jugadoresBeforeMatch = copy.deepcopy(jugadores)

    # ESCRIBO EL INPUT INICIAL
    input = 'archivos/input.txt'
    write(input, jugadores)
    
    numPartidas = 2
    jugadoresPostMatch = []
    for i in range(numPartidas):
        #jugadores[0] = ["Player0", "100", "30"]
        #jugadores[1] = ["Player1", "100", "20"]
        #LEO EL ULTIMO OUTPUT.-------------------------
        jugadores = read(input) 
        jugadores = mergeSortByRating(jugadores)

        #ESCRIBIR TXT------------------------------   
        #input = 'input'+str(p)+'.txt'
        #write(input, jugadores) 

        jugadoresPostMatch = [] #GUARDA LOS QUE SOBRAN SIN GRUPO
        partidas = createPartidas(jugadores, numPlayers, jugadoresPostMatch)

        for p in partidas:
            jugadoresPostMatch = determineWinner(p, jugadoresPostMatch)

        output = 'archivos/output'+str(i)+'.txt'
        print (output)
        input = output #el input de la siguiente iteracion es el output actual
        write(output, jugadoresPostMatch)

    #     print(jugadoresBeforeMatch)
    #     print()
    #     print(jugadoresPostMatch)

    # TERMINARON TODAS LAS PARTIDAS ==> COMPARAMOS
    #print(jugadoresPostMatch)
    jugadoresPostMatch = mergeSortById(jugadoresPostMatch)
    result = 'archivos/result.txt'
    compareResults(jugadoresBeforeMatch, jugadoresPostMatch, result)

main()

----------------------------------------------
#method to sort array
def sortArray(array):
    for i in range(len(array)):
        for j in range(len(array)):
            if array[i] < array[j]:
                aux = array[i]
                array[i] = array[j]
                array[j] = aux
    return array


#method to merge sort array
def mergeSort(array):


    if len(array) > 1:
        mid = len(array) // 2 #quita los decimales
        left = array[:mid]
        right = array[mid:]

        mergeSort(left)
        mergeSort(right)

        i = 0
        j = 0
        k = 0

        while i < len(left) and j < len(right):
            if left[i] < right[j]:
                array[k] = left[i]
                i += 1
            else:
                array[k] = right[j]
                j += 1
            k += 1

        while i < len(left):
            array[k] = left[i]
            i += 1
            k += 1

        while j < len(right):
            array[k] = right[j]
            j += 1
            k += 1

    return array