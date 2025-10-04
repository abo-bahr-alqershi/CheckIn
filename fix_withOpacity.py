#!/usr/bin/env python3
import os
import re
import glob

def fix_withOpacity_in_file(file_path):
    """Fix withOpacity usage in a single file"""
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
        
        # Pattern to match withOpacity calls
        # This handles both simple cases like .withOpacity(0.5) and complex ones like .withOpacity(0.3 + 0.1 * _glowAnimation.value)
        pattern = r'\.withOpacity\s*\(\s*([^)]+)\s*\)'
        
        def replace_withOpacity(match):
            opacity_value = match.group(1).strip()
            return f'.withValues(alpha: {opacity_value})'
        
        new_content = re.sub(pattern, replace_withOpacity, content)
        
        if new_content != content:
            with open(file_path, 'w', encoding='utf-8') as f:
                f.write(new_content)
            print(f"Fixed: {file_path}")
            return True
        return False
    except Exception as e:
        print(f"Error processing {file_path}: {e}")
        return False

def main():
    """Main function to fix all Dart files"""
    # Find all Dart files in the control_panel_app directory
    dart_files = glob.glob('control_panel_app/lib/**/*.dart', recursive=True)
    
    fixed_count = 0
    total_files = len(dart_files)
    
    print(f"Found {total_files} Dart files to process...")
    
    for file_path in dart_files:
        if fix_withOpacity_in_file(file_path):
            fixed_count += 1
    
    print(f"\nFixed {fixed_count} out of {total_files} files")

if __name__ == "__main__":
    main()