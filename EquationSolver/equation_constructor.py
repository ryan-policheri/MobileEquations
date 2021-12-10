import re

def equation_constructor(characters):
    print("Combining characters into equation...")

    characters = [str(character) for character in characters if str(character)!= " "]
    equation = "".join(characters)
    
    #this line removes any trailing operations
    equation = equation.rstrip("+-*/#")
    
    #this line removes any preceding operations
    #limited it to only those which would create syntax issues since +-+8 is valid for example
    equation = equation.lstrip("*/#")
    
    def trim_zeros(m):
        return m.group(1)
        
    equation = re.sub(r"0+([1-9])", trim_zeros, equation)
    
    return equation