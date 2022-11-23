Project TV Time

	Find  at GitHub: https://github.com/aharonlevy/Get-TV-Show-Total-Length
	The project was written by Aharon levy.
	find me at my LinkedIn: https://www.linkedin.com/in/aharon-levy/
	The project makes use of the TVmaze API
	
	With Project TV Time you can find the most time-consuming and,
	the least time-consuming shows to binge to watch.
	
	The project will receive a list of shows from you, find the most recent show 
	with that name from the search, sum up every show watch time, and will return
	to the console the name and length of both the shortest and longest shows.
	
	

	TV Time doesn't require an installation,
	to run it smoothly you should install:
		- Python 3.9.12 or over
		- C# 10.0
		-.NET 6
	How to use:
		1. declare an environment variable called GET_TVSHOW_TOTAL_LENGTH_BIN containing the path to 'GetTvShowTotalLength.exe'
		2. make a .txt file containing the shows you want to check every show should be in a different line.
		3. on your CMD get to the main folder of the project and run "python tv-time.py [path to txt file]"
	
	Thanks for your interest and engagement! This project was made as a part of my learning process
	if you find some part of it faulty or not functional I would be more than happy to hear from you VIA GitHub. 



	Known problems:
		some shows had in the "airdate" data as an empty string ("") instead of a date. It broke the deserialization.
		I treated them as non-zero exit code shows
	 
		  
		