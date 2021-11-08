import sys
import json
import random

import constants
from Equation import Equation

def solveEquation(filePath):
    print("Doing complex things...")
    sourceEquation = str(random.randint(1, 20)) + " + " + str(random.randint(3, 12)) + " = ";
    print("The equation is " + sourceEquation)
    solution = str(random.randint(5, 15))
    print("The solution is " + solution)
    solvedEquation = sourceEquation + solution
    
    equation = Equation(sourceEquation, solution, solvedEquation)
    return equation


args = sys.argv

if len(args) < 3: #evidently the first argument has the program path in it automatically
    print("You called me without any arguments...")
    sys.exit(constants.EXIT_ARGS_INCOMPLETE)

inputFile = args[1] #so we start by grabbing the second arg
outputFile = args[2]

inputData = ""
try:
    with open(inputFile) as file:
        fileContent = file.read()
        inputData = json.loads(fileContent)
except FileNotFoundError:
    print("Input file not found")
    sys.exit(constants.EXIT_ARGS_FILE_NOT_FOUND)
except Exception as ex:
    print("Unhandled error")
    print(ex)
    
imagePath = inputData["imagePath"]
equation = solveEquation(imagePath)
asJson = equation.toJSON()

with open(outputFile, 'w') as file:
    file.write(asJson)

sys.exit(0)