$(function(){
	for(var i=0;i<4;i++)
	{
		$(`#aaa tbody tr:nth(${i}) td:nth(0)`).on('click',(e)=>{
			alert($('#aaa tbody tr').index(e.target.parentNode));
			const ti= $('#aaa tbody tr').index(e.target.parentNode);
			$('#list').appendTo(`#aaa tbody tr:nth(${ti}) td:nth(0)`);
			$('#list').css('display','block');
			$('#list').css('z-index','100');
		});
	}
});
