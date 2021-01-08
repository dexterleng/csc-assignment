# CSC Assignment

# Arrangement of files
Each task is separated into its respective folders. Within each folder, they will contain:
- readme.pdf (and identical source markdown file)
- source code of solution to the task
- images and diagrams related to the task

The individual readme documents will contain important info on how to setup and use the project as well as housing the deliverables for the task.

# Required steps to setup on all projects

Once Visual Studio is open, navigate to `Tools > NuGet Package Manager > Manage NuGet Packages for Solution`
![](images/initial/1.png)

Click restore on the banner to restore missing nuget packages for the project
![](images/initial/2.png)

Upon running and there is an error about `csc.exe`, clean project (rebuilding alone does not work) then build and run again
