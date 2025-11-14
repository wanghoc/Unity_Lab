# Hướng Dẫn Sửa Lỗi Dự Án Unity 3D Survival Game

## Các Lỗi Đã Được Sửa Tự Động

✅ **Đã sửa:** Lỗi `Unity.Mathematics` - Đã thay thế `quaternion` bằng `Quaternion` trong các file:
- ResourceFruitTree.cs
- Resources.cs
- ResourceStone.cs
- Bed.cs

## Các Package Unity Cần Cài Đặt

### 1. TextMeshPro (TMPro)
**Lỗi:** `The type or namespace name 'TMPro' could not be found`

**Cách sửa:**
1. Mở Unity Editor
2. Vào menu: `Window` → `Package Manager`
3. Tìm kiếm "TextMeshPro"
4. Click vào package và nhấn `Install`
5. Khi xuất hiện cửa sổ "Import TMP Essentials", nhấn `Import`

### 2. Input System
**Lỗi:** `The type or namespace name 'InputSystem' does not exist in the namespace 'UnityEngine'`

**Cách sửa:**
1. Mở Unity Editor
2. Vào menu: `Window` → `Package Manager`
3. Chọn "Unity Registry" từ dropdown menu
4. Tìm kiếm "Input System"
5. Click vào package và nhấn `Install`
6. Khi Unity hỏi restart, nhấn `Yes`

**Quan trọng:** Sau khi cài Input System, cần cấu hình:
1. Vào `Edit` → `Project Settings` → `Player`
2. Tìm phần "Active Input Handling"
3. Chọn "Both" hoặc "Input System Package (New)"
4. Restart Unity Editor

### 3. Unity UI
**Lỗi:** `The type or namespace name 'UI' does not exist in the namespace 'UnityEngine'`

**Cách sửa:**
Unity UI thường được cài sẵn, nhưng nếu vẫn bị lỗi:
1. Vào `Window` → `Package Manager`
2. Tìm "Unity UI" hoặc "com.unity.ugui"
3. Đảm bảo package đã được cài đặt

## Kiểm Tra Sau Khi Cài Đặt

Sau khi cài đặt tất cả các package:

1. Đợi Unity import xong các package
2. Vào `Window` → `TextMeshPro` → `Import TMP Essential Resources` (nếu chưa import)
3. Kiểm tra Console không còn lỗi compile nữa
4. Build lại project để đảm bảo mọi thứ hoạt động

## Các File Đã Sử Dụng Package Nào

### TextMeshPro (TMPro):
- BuildingRecipeUI.cs
- InteractionManager.cs
- Inventory.cs
- CraftingRecipeUI.cs
- ItemSlotUI.cs

### Input System:
- BuildingWindow.cs
- InteractionManager.cs
- Bed.cs
- EquipBuildingKit.cs
- EquipManager.cs
- Inventory.cs
- PlayerController.cs

### Unity UI:
- BuildingRecipeUI.cs (Image)
- Menu.cs (RawImage)
- Bed.cs (RawImage)
- PlayerNeeds.cs (Image)
- CraftingRecipeUI.cs (Image)
- DamageIndicator.cs (Image)
- ItemSlotUI.cs (Button, Image, Outline)

## Lưu Ý

- Đảm bảo bạn đang sử dụng Unity phiên bản 2019.4 trở lên để hỗ trợ đầy đủ các package này
- Input System yêu cầu restart Unity Editor
- TextMeshPro cần import TMP Essential Resources để hoạt động đúng
- Nếu vẫn còn lỗi, thử xóa thư mục `Library` trong project và để Unity rebuild

## Liên Hệ

Nếu vẫn gặp vấn đề sau khi làm theo hướng dẫn, vui lòng kiểm tra:
1. Phiên bản Unity của bạn
2. Console log chi tiết
3. Package Manager có cài đặt thành công không

---
**Tóm tắt:** Cài đặt 3 package chính: TextMeshPro, Input System, và đảm bảo Unity UI đã có sẵn.
