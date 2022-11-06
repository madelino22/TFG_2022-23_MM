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
    jugadores = []
    for i in range(num):
        rating = random.randint(0, 3000)

        proporcion = 1 - (rating / 3000) # si es muy bueno = 0, malo = 1
        k = 10 + 30 * proporcion 
        k = int(k)
        if k >= 11 & k <= 39:
            k = k + (random.randint(-100, 100) / 100)

       # k = random.randint(10, 40)
        jugadores.append([str(i), str(rating),str(k)])
    return jugadores

#---------------------------------------------------------------------------------------------
def write(name, jugadores):
    archivo = open(name, "w")
    for j in jugadores:
        archivo.write("Player" + j[0] + " " + j[1] + " " + j[2] + "\n")

    archivo.close()


def mergeSortByRating(array):
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
            if int(left[i][0]) < int(right[j][0]):
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
    "jugadores must have been organized through MergeSort. Only 6 members"
   
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
    ini = 0
    numPlayersInMatch = 6
    fin = numPlayersInMatch
    partidas = []
    while fin <= numPlayers: #FORMAMOS GRUPOS
        print()
        partida = createTeams(jugadores[ini:fin])
        partidas.append(partida)
        ini += numPlayersInMatch
        fin += numPlayersInMatch

    while ini < numPlayers: #GUARDAMOS LOS QUE NO TIENEN GRUPO PARA LUEGO
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
    if len(jugadoresBeforeMatch) != len(jugadoresPostMatch):
        print("Error en compareReults diff len")
        return
    
    archivo = open(results, "w")

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
    archivo = 'input.txt'
    numPlayers = 13
    #jugadores = read(archivo)
    #GENERAR STATS----------------------------
    jugadores = generatePlayers(numPlayers)
    jugadoresBeforeMatch = copy.deepcopy(jugadores)
   
    #jugadores[0] = ["Player0", "100", "30"]
    #jugadores[1] = ["Player1", "100", "20"]
    jugadores = mergeSortByRating(jugadores)
    #ESCRIBIR TXT------------------------------
    
    write(archivo, jugadores)

    jugadoresPostMatch = [] #GUARDA LOS QUE SOBRAN SIN GRUPO
    partidas = createPartidas(jugadores, numPlayers, jugadoresPostMatch)

    for p in partidas:
        jugadoresPostMatch = determineWinner(p, jugadoresPostMatch)

    write('output.txt', jugadoresPostMatch)

    jugadoresPostMatch = mergeSortById(jugadoresPostMatch)
#     print(jugadoresBeforeMatch)
#     print()
#     print(jugadoresPostMatch)
    compareResults(jugadoresBeforeMatch, jugadoresPostMatch, 'result.txt')

main()