import re

def equation_constructor(characters):
    print("Combining characters into equation...")

    mappings = {
        0: "0",
        1: "1",
        2: "2",
        3: "3",
        4: "4",
        5: "5",
        6: "6",
        7: "7",
        8: "8",
        9: "9",
        10: "+",
        11: "-",
        12: "*",
        13: "#",
        14: "/"
    }

    characters = [mappings[character] for character in characters]
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