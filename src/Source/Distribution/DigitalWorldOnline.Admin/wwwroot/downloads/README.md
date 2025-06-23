# Downloads Directory

This directory contains game client installers and updates that can be downloaded by authenticated users.

## Structure

```
Downloads/
├── x64/          # 64-bit installers and updates
├── x86/          # 32-bit installers and updates
└── README.md     # This file
```

## Security

- All downloads require user authentication
- File access is validated and sanitized
- Only specific file types are allowed (.zip, .exe, .msi)
- Path traversal attacks are prevented
- File size limits are enforced (500MB max)

## Supported File Types

- `.zip` - Compressed archives
- `.exe` - Windows executables
- `.msi` - Windows installer packages

## Adding New Downloads

1. Place files in the appropriate architecture folder (x64 or x86)
2. Ensure file names contain only alphanumeric characters, dots, dashes, and underscores
3. Files will be automatically detected by the download service
4. Check logs for any validation errors

## File Naming Convention

- Use descriptive names: `GameClient_v1.0.0_x64.zip`
- Include version information when applicable
- Use architecture suffix for clarity
- Avoid special characters and spaces
