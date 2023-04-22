import pandas as pd
import numpy as np
import scipy.io as scp
import scipy.optimize as scy
import matplotlib.pyplot as plt
import copy
import math
import seaborn as sns
import json
import random

def bubbleSort (array):
    # Establecemos la variable intercambio en True para entrar en el bucle al menos una vez
    intercambio = True
    while intercambio:
        intercambio = False
        for i in range(len(array) - 1):
            if array[i]["eloRanking"] > array[i + 1]["eloRanking"]:
                #Intercambiamoslos datos
                array[i], array [i + 1] = array[i + 1], array[i]
                # Cambiamos a True la variable ya que ha habido un intercambio
                intercambio = True

def createPlayer(variableNames):
    persona = {}  # Esto es un diccionario vacío
    for i in range(len(variableNames)):
        if(variableNames[i] == "eloK"):
            persona[variableNames[i]] = 40
        elif(variableNames[i] == "eloRanking"):
            persona[variableNames[i]] = 1500
        else:
            persona[variableNames[i]] = 0
    
    # #Variables extra determinar como es cada jugador
    # persona["Calidad"] = random.randrange(40, 60) #Como de bueno es el jugador al empezar
    # persona["Ratio de mejora"] = random.randrange(10, 100) #Cuanto mejora conforme juega

    return persona

def updatePlayer(player, kills, deaths, healMe, healOther, bulletsHittedEnemy, winner, winProb=0): #perder 0 ganador 1 empate 0.5
    player["gamesPlayed"] += 1
    if(winner == 0):
        player["loses"] += 1
    elif(winner == 1):
        player["wins"] += 1
    else:
        player["draws"] += 1

    if(player["gamesPlayed"] > 10):
        if(player["eloRanking"] < 2100):                                                                                                                                                                  
            player["eloK"] = 32
        elif(player["eloRanking"] > 2400):
            player["eloK"] = 16
        else:
            player["eloK"] = 24

    player["deaths"] += deaths
    player["kills"] += kills
    if(deaths != 0):
        player["killsDeathsRatioAverage"] = player["kills"]/player["deaths"]
    else:
        player["killsDeathsRatioAverage"] += player["kills"]

    player["healedMyLife"] += healMe
    player["healedOthersLife"] += healOther

    player["damageInflicted"] += bulletsHittedEnemy * 250
    player["damageReceived"] += deaths * 250 + healMe #Lo que hicieron pa matarme y lo que me curaron
    player["dps"] = (player["damageInflicted"] / player["gamesPlayed"]) / 120

    failBulletRatio = random.randrange(10, 20)
    failBulletRatio = failBulletRatio/10
    player["totalShots"] += bulletsHittedEnemy * failBulletRatio



    player["eloRanking"] = calculateElo(player["eloRanking"], player["eloK"],winner ,winProb)


    return player


def calculateElo(eloRanking, eloK, SA, probWin):
    
    newElo = eloRanking + eloK *(SA - probWin)

    if(newElo < 0): newElo = 0
    elif (newElo > 3000): newElo = 3000
    
    return newElo
    
    

def match(playerRed1, playerRed2, playerBlue1, playerBlue2):

    sumELO = playerRed1["eloRanking"] + playerRed2["eloRanking"] + playerBlue1["eloRanking"] + playerBlue2["eloRanking"]

    redWinProb = 0
    blueWinProb = 0

    if(sumELO == 0):
        redWinProb = 0.5
        blueWinProb = 0.5
    else:
        redWinProb = ((playerRed1["eloRanking"] + playerRed2["eloRanking"])/sumELO)
        blueWinProb = 1 - redWinProb


    #Dejo un porcentaje de empate que sea invesamente proporcional a sus posibilidades de ganar
    redWinProbAjust = redWinProb - blueWinProb * 0.1
    blueWinProbAjust = blueWinProb - redWinProb * 0.1
    if redWinProbAjust < 0:
        redWinProbAjust = 0
    if blueWinProbAjust < 0:
        blueWinProbAjust = 0
    drawProb = blueWinProbAjust - redWinProbAjust * 0.1 + redWinProbAjust - blueWinProbAjust * 0.1

    winner = random.randrange(0, 100)

    if(winner < drawProb): #Empate
        points = random.randrange(0, 10)

                 #Actualizar partida
        player1Heal = random.randrange(0, 20)
        player2Heal = random.randrange(0, 20)

        player3Heal = random.randrange(0, 20)
        player4Heal = random.randrange(0, 20)

        kills1Player = 0
        kills2Player = 0
        if(points!= 0):
            kills1Player = random.randrange(0, points)
            kills2Player = points - kills1Player

        deaths1Player = 0
        deaths2Player = 0
        if(points!= 0):
            deaths1Player = random.randrange(0, points)
            deaths2Player = points - deaths1Player
        
        totalDamage = points * 5 + player3Heal + player4Heal
        hittedPlayer1 = 0 
        if(totalDamage != 0):
            random.randrange(0, totalDamage)

        playerRed1 = updatePlayer(playerRed1, kills1Player, deaths1Player, player2Heal*250, player1Heal*250, hittedPlayer1, 0.5, redWinProb)
        playerRed2 = updatePlayer(playerRed2, kills2Player, deaths2Player, player1Heal*250, player2Heal*250, totalDamage - hittedPlayer1, 0.5, redWinProb)

        kills3Player = 0
        kills4Player = 0
        if(points!= 0):
            kills3Player = random.randrange(0, points)
            kills4Player = points - kills3Player
        
        deaths3Player = 0
        deaths4Player = 0
        if(points!= 0):
            deaths3Player = random.randrange(0, points)
            deaths4Player = points - deaths3Player
        
        totalDamage = points * 5 + player1Heal + player2Heal
        hittedPlayer3 = 0 
        if(totalDamage != 0):
            random.randrange(0, totalDamage)

        playerBlue1 = updatePlayer(playerBlue1, kills3Player, deaths3Player, player3Heal*250, player4Heal*250, hittedPlayer3, 0.5, blueWinProb)
        playerBlue2 = updatePlayer(playerBlue2, kills4Player, deaths4Player, player4Heal*250, player3Heal*250, totalDamage - hittedPlayer3, 0.5, blueWinProb)

    elif (winner < drawProb): #Ganra rojo
        pointsRed = random.randrange(1, 10)
        pointsBlue = 0
        if(pointsRed != 1):
            pointsBlue = random.randrange(0, pointsRed - 1)

         #Actualizar partida
        player1Heal = random.randrange(0, 20)
        player2Heal = random.randrange(0, 20)

        player3Heal = random.randrange(0, 20)
        player4Heal = random.randrange(0, 20)

        kills1Player = 0
        kills2Player = 0
        if(pointsRed!= 0):
            kills1Player = random.randrange(0, pointsRed)
            kills2Player = pointsRed - kills1Player

        deaths1Player = 0
        deaths2Player = 0
        if(pointsBlue!= 0):
            deaths1Player = random.randrange(0, pointsBlue)
            deaths2Player = pointsBlue - deaths1Player
        
        totalDamage = pointsRed * 5 + player3Heal + player4Heal
        hittedPlayer1 = 0 
        if(totalDamage != 0):
            random.randrange(0, totalDamage)

        playerRed1 = updatePlayer(playerRed1, kills1Player, deaths1Player, player2Heal*250, player1Heal*250, hittedPlayer1, 1, redWinProb)
        playerRed2 = updatePlayer(playerRed2, kills2Player, deaths2Player, player1Heal*250, player2Heal*250, totalDamage - hittedPlayer1, 1, redWinProb)

        kills3Player = 0
        kills4Player = 0
        if(pointsBlue!= 0):
            kills3Player = random.randrange(0, pointsBlue)
            kills4Player = pointsBlue - kills3Player
        
        deaths3Player = 0
        deaths4Player = 0
        if(pointsRed!= 0):
            deaths3Player = random.randrange(0, pointsRed)
            deaths4Player = pointsRed - deaths3Player
        
        totalDamage = pointsBlue * 5 + player1Heal + player2Heal
        hittedPlayer3 = 0 
        if(totalDamage != 0):
            random.randrange(0, totalDamage)

        playerBlue1 = updatePlayer(playerBlue1, kills3Player, deaths3Player, player3Heal*250, player4Heal*250, hittedPlayer3, 0, blueWinProb)
        playerBlue2 = updatePlayer(playerBlue2, kills4Player, deaths4Player, player4Heal*250, player3Heal*250, totalDamage - hittedPlayer3, 0, blueWinProb)
    else: #Gana azul
        pointsBlue = random.randrange(1, 10)
        pointsRed = 0
        if(pointsBlue != 1):
            pointsRed = random.randrange(0, pointsBlue - 1)

        #Actualizar partida
        player1Heal = random.randrange(0, 20)
        player2Heal = random.randrange(0, 20)

        player3Heal = random.randrange(0, 20)
        player4Heal = random.randrange(0, 20)

        kills1Player = 0
        kills2Player = 0
        if(pointsRed!= 0):
            kills1Player = random.randrange(0, pointsRed)
            kills2Player = pointsRed - kills1Player

        deaths1Player = 0
        deaths2Player = 0
        if(pointsBlue!= 0):
            deaths1Player = random.randrange(0, pointsBlue)
            deaths2Player = pointsBlue - deaths1Player
        
        totalDamage = pointsRed * 5 + player3Heal + player4Heal
        hittedPlayer1 = 0 
        if(totalDamage != 0):
            random.randrange(0, totalDamage)

        playerRed1 = updatePlayer(playerRed1, kills1Player, deaths1Player, player2Heal*250, player1Heal*250, hittedPlayer1, 0, redWinProb)
        playerRed2 = updatePlayer(playerRed2, kills2Player, deaths2Player, player1Heal*250, player2Heal*250, totalDamage - hittedPlayer1, 0, redWinProb)

        kills3Player = 0
        kills4Player = 0
        if(pointsBlue!= 0):
            kills3Player = random.randrange(0, pointsBlue)
            kills4Player = pointsBlue - kills3Player
        
        deaths3Player = 0
        deaths4Player = 0
        if(pointsRed!= 0):
            deaths3Player = random.randrange(0, pointsRed)
            deaths4Player = pointsRed - deaths3Player
        
        totalDamage = pointsBlue * 5 + player1Heal + player2Heal
        hittedPlayer3 = 0 
        if(totalDamage != 0):
            random.randrange(0, totalDamage)

        playerBlue1 = updatePlayer(playerBlue1, kills3Player, deaths3Player, player3Heal*250, player4Heal*250, hittedPlayer3, 1, blueWinProb)
        playerBlue2 = updatePlayer(playerBlue2, kills4Player, deaths4Player, player4Heal*250, player3Heal*250, totalDamage - hittedPlayer3, 1, blueWinProb)
     
    return playerRed1, playerRed2, playerBlue1, playerBlue2

def main():

    NUMPLAYERS = 2000
    PLAYERSGAME = 4
    TOTALROUNDS = 500
    MAXELO = 3000
    #-------------RECOGIDA DE DATOS----------------
 
    with open('./FireBase/PruebasMM1.json', encoding="utf-8") as user_files:
        file_contents = user_files.read()

    data = json.loads(file_contents)
    
    dic = { }

    #INICIALIZACION DATA: PUEDE DAR ERROR SI NO ESTE EL USUARIO, CAMBIAR POR OTRO QUE ESTE
    variableNames = []
    for j in data['User']["Este es mi usuario"]: 
        if(j != "userName" and j != "zzlastGameSaved" and j != "email"):
            dic[j] = []
            variableNames = np.concatenate((variableNames, [j]))

    createdPlayers = []

    #CREAR JUGADORES 
    for i in range(NUMPLAYERS):
        createdPlayers = np.concatenate((createdPlayers, [createPlayer(variableNames)] ))

    #GENERA PARTIDAS
    for i in range(TOTALROUNDS): #Numero de veces que van a jugar
        bubbleSort(createdPlayers)
        for j in range(int(NUMPLAYERS/PLAYERSGAME)): #Numero de partidas
            createdPlayers[j*PLAYERSGAME], createdPlayers[j*PLAYERSGAME + 3], createdPlayers[j*PLAYERSGAME + 2], createdPlayers[j*PLAYERSGAME + 1] = match(createdPlayers[j*PLAYERSGAME], createdPlayers[j*PLAYERSGAME +3], createdPlayers[j*PLAYERSGAME + 2], createdPlayers[j*PLAYERSGAME + 1])
        if(i%50 == 0):
            print("Ronda: ", i)

    playersELOX = np.zeros(MAXELO + 1)
    playersELOY = np.zeros(MAXELO + 1)

    for i in range(len(playersELOY)):
        playersELOX[i] = i

    for i in range(NUMPLAYERS):
        indice = int(createdPlayers[i]["eloRanking"])
        playersELOY[indice] =  playersELOY[indice] + 1

        
    plt.plot(playersELOX, playersELOY)
    
    #.............VISUALIZACION-----------------------

    plt.show()

main()