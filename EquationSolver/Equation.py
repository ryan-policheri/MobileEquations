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
        return "x = \\frac{-b \\pm \\sqrt{b^2-4ac}}{2a}"