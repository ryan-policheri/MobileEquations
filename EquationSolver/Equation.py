import json

class Equation:
    def __init__(self, equation, solution, solvedEquation):
        self.equation = equation
        self.solution = solution
        self.solvedEquation = solvedEquation
        self.laTeX = self.buildLaTeX()
        
    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__, sort_keys=True, indent=4)

    def buildLaTeX(self):
        return "foobar" #not sure what valid LaTeX looks like