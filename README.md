## Speech to Text Bot with the following commands:  
Hello  
How are you  
What time is it  
Stop talking  
Stop listening  
Wake up  
Show commands  
Hide commands
Ask questions:
 - Asks "For which of your pets do you want to schedule an appointment?" and depending on your answer, it tells you back which pet you've selected
 
Uses the Damerau-Levenshtein distance algorithm to find the number of steps it would take to transform your answer into one of the expected answers using
insertion, deletion, subtitution and transposition

The project is divided into three parts:
 - The STT_Bot, the main bot that has the above mentioned commands
 - The STT_External, the web service that communicates with the DB and selects the possible answers, depending on the question asked
 - The STT_Internal, the web service that matches the phonetically matches the asnwer with the list of expected answers, with a maximum distance allowed to be considered a match
