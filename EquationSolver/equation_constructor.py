def equation_constructor(characters):
    print("Combining characters into equation...")

    characters = [str(character) for character in characters]
    equation = " ".join(characters)
    
    return equation