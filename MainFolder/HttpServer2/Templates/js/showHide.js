function change_password_visibility(icon, input_id) {
    let input = document.getElementById(input_id);
    let input_type = input.getAttribute('type');
    if (input_type == 'text'){
        icon.classList.remove('hide');
        input.setAttribute('type', 'password');
    } else if (input_type == 'password') {
        icon.classList.add('hide');
		input.setAttribute('type', 'text');
    } else{
        console.error('Не правильно задан input_id');
    }
}