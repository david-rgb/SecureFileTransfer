export function generatePasscode(length = 6): string {
	const charset = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
	let result = "";
	for (let i = 0; i < length; i++) {
		const index = Math.floor(Math.random() * charset.length);
		result += charset[index];
	}
	return result;
}