#!/usr/bin/env python3
# replace_colors.py

import os
import re
import sys
from pathlib import Path
from typing import List, Tuple
import argparse
import shutil
from datetime import datetime

class ColorReplacer:
    def __init__(self, project_path: str, backup: bool = True):
        self.project_path = Path(project_path)
        self.backup = backup
        self.backup_dir = None
        self.files_processed = 0
        self.replacements_made = 0
        self.errors = []
        
        # Flutter file extensions to process
        self.extensions = ['.dart']
        
        # Directories to skip
        self.skip_dirs = {
            '.git', '.idea', '.vscode', 'build', '.dart_tool', 
            'android/build', 'ios/build', 'macos/build', 'windows/build',
            'linux/build', 'web/build', '.flutter-plugins-dependencies'
        }
        
        # Files to skip
        self.skip_files = {
            'app_colors.dart', 'app_colors_light.dart', 'app_theme.dart',
            'pubspec.lock', '.packages'
        }

    def create_backup(self):
        """Create backup of the entire project"""
        if not self.backup:
            return
            
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        self.backup_dir = self.project_path.parent / f"backup_{self.project_path.name}_{timestamp}"
        
        print(f"📦 Creating backup at: {self.backup_dir}")
        shutil.copytree(self.project_path, self.backup_dir)
        print("✅ Backup created successfully\n")

    def should_skip_path(self, path: Path) -> bool:
        """Check if path should be skipped"""
        # Check if any parent directory should be skipped
        for parent in path.parents:
            if parent.name in self.skip_dirs:
                return True
        
        # Check if file should be skipped
        if path.name in self.skip_files:
            return True
            
        return False

    def find_dart_files(self) -> List[Path]:
        """Find all Dart files in the project"""
        dart_files = []
        
        for ext in self.extensions:
            for file_path in self.project_path.rglob(f"*{ext}"):
                if not self.should_skip_path(file_path):
                    dart_files.append(file_path)
        
        return dart_files

    def replace_in_file(self, file_path: Path) -> int:
        """Replace AppColors with AppTheme in a single file"""
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
            
            original_content = content
            replacements = 0
            
            # Pattern 1: Replace AppColors class references
            pattern1 = r'\bAppColors\.'
            replacement1 = 'AppTheme.'
            content, count1 = re.subn(pattern1, replacement1, content)
            replacements += count1
            
            # Pattern 2: Replace AppColorsLight class references
            pattern2 = r'\bAppColorsLight\.'
            replacement2 = 'AppTheme.'
            content, count2 = re.subn(pattern2, replacement2, content)
            replacements += count2
            
            # Pattern 3: Update import statements
            import_patterns = [
                (r"import\s+['\"].*app_colors\.dart['\"];?", 
                 "import 'package:your_app/core/theme/app_theme.dart';"),
                (r"import\s+['\"].*app_colors_light\.dart['\"];?", 
                 "import 'package:your_app/core/theme/app_theme.dart';"),
            ]
            
            for pattern, replacement in import_patterns:
                if re.search(pattern, content):
                    content = re.sub(pattern, replacement, content)
                    replacements += 1
            
            # Pattern 4: Handle special cases in comments
            # Keep comments as is, but update if they're example code
            comment_pattern = r'(//.*?)(AppColors|AppColorsLight)\.'
            content = re.sub(comment_pattern, r'\1AppTheme.', content)
            
            # Only write if changes were made
            if content != original_content:
                with open(file_path, 'w', encoding='utf-8') as f:
                    f.write(content)
                
                print(f"✓ {file_path.relative_to(self.project_path)}: {replacements} replacements")
            
            return replacements
            
        except Exception as e:
            self.errors.append(f"Error in {file_path}: {str(e)}")
            return 0

    def add_theme_initialization(self):
        """Add theme initialization to main.dart if needed"""
        main_file = self.project_path / 'lib' / 'main.dart'
        
        if not main_file.exists():
            print("⚠️  main.dart not found, skipping initialization code")
            return
        
        try:
            with open(main_file, 'r', encoding='utf-8') as f:
                content = f.read()
            
            # Check if AppTheme is already initialized
            if 'AppTheme.init' in content:
                print("ℹ️  Theme initialization already exists in main.dart")
                return
            
            # Add initialization in build method of main app
            init_code = """
    // Initialize theme
    AppTheme.init(context);
"""
            
            # Find MyApp or MaterialApp widget build method
            pattern = r'(Widget build\(BuildContext context\) \{)'
            
            if re.search(pattern, content):
                content = re.sub(
                    pattern,
                    r'\1' + init_code,
                    content,
                    count=1
                )
                
                with open(main_file, 'w', encoding='utf-8') as f:
                    f.write(content)
                
                print("✅ Added theme initialization to main.dart")
            else:
                print("⚠️  Could not find build method in main.dart")
                
        except Exception as e:
            print(f"⚠️  Error updating main.dart: {str(e)}")

    def run(self):
        """Execute the replacement process"""
        print("🚀 Starting AppColors to AppTheme conversion")
        print(f"📁 Project path: {self.project_path}\n")
        
        # Create backup
        self.create_backup()
        
        # Find all Dart files
        print("🔍 Searching for Dart files...")
        dart_files = self.find_dart_files()
        print(f"📄 Found {len(dart_files)} Dart files\n")
        
        # Process each file
        print("🔄 Processing files...")
        for file_path in dart_files:
            replacements = self.replace_in_file(file_path)
            if replacements > 0:
                self.files_processed += 1
                self.replacements_made += replacements
        
        # Add theme initialization
        self.add_theme_initialization()
        
        # Print summary
        print("\n" + "="*50)
        print("✅ CONVERSION COMPLETE!")
        print("="*50)
        print(f"📊 Files processed: {self.files_processed}")
        print(f"🔄 Total replacements: {self.replacements_made}")
        
        if self.backup_dir:
            print(f"💾 Backup location: {self.backup_dir}")
        
        if self.errors:
            print(f"\n⚠️  Errors encountered: {len(self.errors)}")
            for error in self.errors[:5]:  # Show first 5 errors
                print(f"   - {error}")
        
        print("\n📝 Next steps:")
        print("1. Update 'your_app' in import statements to your actual package name")
        print("2. Test the application thoroughly")
        print("3. Delete the backup after confirming everything works")

def main():
    parser = argparse.ArgumentParser(
        description='Replace AppColors with AppTheme in Flutter project'
    )
    parser.add_argument(
        'project_path',
        help='Path to Flutter project root directory'
    )
    parser.add_argument(
        '--no-backup',
        action='store_true',
        help='Skip creating backup'
    )
    parser.add_argument(
        '--dry-run',
        action='store_true',
        help='Show what would be changed without making changes'
    )
    
    args = parser.parse_args()
    
    # Validate project path
    project_path = Path(args.project_path)
    if not project_path.exists():
        print(f"❌ Error: Project path does not exist: {project_path}")
        sys.exit(1)
    
    if not (project_path / 'pubspec.yaml').exists():
        print(f"❌ Error: Not a Flutter project (pubspec.yaml not found)")
        sys.exit(1)
    
    # Run replacer
    replacer = ColorReplacer(
        project_path=args.project_path,
        backup=not args.no_backup
    )
    
    replacer.run()

if __name__ == "__main__":
    main()