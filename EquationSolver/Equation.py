import json

class Equation:
    def __init__(self, equation, solution, solvedEquation):
        self.equation = equation
        self.solution = solution
        self.solvedEquation = solvedEquation
        
    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__, sort_keys=True, indent=4)