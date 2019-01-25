/*
 * 
 * Auto resize textarea based on lines of input
 *
 * @params: 
 *	step: function({count: 0}) // callback each time row count increases
 *
 */
$.fn.autoResize = function(obj) {
	if($(this).prop('tagName') == 'TEXTAREA') {
		
		$(this).css("overflow-y", "hidden");
		$(this).css("resize", "none");

		$(this).keyup(function(){
			arr = $(this).val().split("\n");
			$(this).attr("rows", arr.length);	
		
			if(obj && "step" in obj) {
				obj.step({count: arr.length-1});
			}
		});

	}
}