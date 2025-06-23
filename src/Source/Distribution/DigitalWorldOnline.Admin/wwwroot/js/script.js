function downloadFile(fileName, mimeType, base64Content) {
    const blob = new Blob([base64Content], { type: mimeType });
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
    window.URL.revokeObjectURL(link.href);
}