
AUTHOR: Benjamin Selby
DATE: 2021/11/23

The objective of this system is to enable a user to define rectangles on a map which denote locations. 
The purpose will be to provide staff with location guidance e.g. when they are locating a student. 

A user can select a rectangle on the map and save it to a database table along with the ID of 
a room (room IDs are obtained from the Synergy Timetable data table). 

In order to deal with image scaling, the location rectangle co-ordinates and size values are stored 
as PROPORTIONS of the image width & height. So, instead of storing X=123 pixels, the value X=0.07
will be stored, indicating that the X value is 0.07 x the width of the image. The rectangles can 
therefore be overlaid on the map regardless of its display size. 

This tool is pretty basic and not designed to be scaleable because it's use will be very limited 
in our organisation. 

======================================================================================================
v1.0.0
======================================================================================================

Initial build. 


TODO: 

	- 
