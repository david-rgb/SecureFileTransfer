export function isTokenExpired(token: string): boolean {
	try {
		const payload = JSON.parse(atob(token.split('.')[1]));
		const expiry = payload.exp * 1000;
		return Date.now() > expiry;
	} catch {
		return true;
	}
}

export function logout() {
	localStorage.removeItem('jwt');
	localStorage.removeItem("token");
	window.location.href = '/admin/login';
}
