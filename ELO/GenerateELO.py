import re
import random

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

def main():
    print("Hello World!")
    #LEER TXT----------------------------------
    archivo = 'input.txt'
    numPlayers = 10
    #jugadores = read(archivo)
    #GENERAR STATS----------------------------
    jugadores = generatePlayers(numPlayers)
    #ESCRIBIR TXT------------------------------
    write(archivo, jugadores)

main()