export function getEmailHtml(name: string, link: string, passcode?: string, message?: string) {
	return `
		<div style="font-family: sans-serif; padding: 20px; color: #333;">
			<h2>Hello ${name},</h2>
			<p>You’ve received secure files via <strong>Secure File Sender</strong>.</p>
			<p><a href="${link}" style="color: #1a73e8;">Click here to download the files</a></p>
			${passcode ? `<p><strong>Passcode:</strong> ${passcode}</p>` : ""}
			${message ? `<div style="margin-top: 20px;"><strong>Message from sender:</strong><p>${message.replace(/\n/g, "<br>")}</p></div>` : ""}
			<p style="margin-top: 40px;">This link will expire in a few days.</p>
			<p>— The Secure File Sender Team</p>
		</div>
	`;
}
