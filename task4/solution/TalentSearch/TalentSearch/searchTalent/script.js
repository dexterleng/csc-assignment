
const spinner = $("#spinner").addClass('hide');

$(document)
	.ajaxStart(function ()
	{
		spinner.removeClass('hide');
	})
	.ajaxStop(function ()
	{
		spinner.addClass('hide');
	});



$('#search').keyup($.debounce(250, true, function ()
{
    //get data from json file
    var urlForJson = "data.json";


    //get data from Restful web Service in development environment
    //var urlForJson = "/api/talents";

    //get data from Restful web Service in production environment
    //var urlForJson= "http://csc123.azurewebsites.net/api/talents";

	//var urlForJson = "";

    //Url for the Cloud image hosting
	var urlForCloudImage = "https://res.cloudinary.com/bruhmomento/image/upload/v1609302453/TalentSearch/";

    var searchField = $('#search').val();
	var myExp = new RegExp(searchField, "i");

	const getJson = () => {
		$.getJSON(urlForJson, function (data) {
			var output = '<ul class="searchresults">';
			$.each(data, function (key, val) {
				//for debug
				console.log(data);
				if ((val.Name.search(myExp) != -1) ||
				(val.Bio.search(myExp) != -1)) {
					output += '<li>';
					output += '<h2>' + val.Name + '</h2>';
					//get the absolute path for local image
					//output += '<img src="images/'+ val.ShortName +'_tn.jpg" alt="'+ val.Name +'" />';

					//get the image from cloud hosting
					output += '<img src=' + urlForCloudImage + val.ShortName + "_tn.jpg alt=" + val.Name + '" />';
					output += '<p>' + val.Bio + '</p>';
					output += '</li>';
				}
			});
			output += '</ul>';
			$('#update').html(output);
		})
		.fail(function () {
			alert("failed to fetch, retrying...");
			setTimeout(getJson, 2500);
		})
	};

	getJson();

}));
