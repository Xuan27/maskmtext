; Auto-load MaskMText plugin for Civil 3D
; Place this file in a Support File Search Path or in the user's roaming profile

(defun s::startup ()
  (command "._NETLOAD" "\\\\YourServer\\Civil3D\\Plugins\\MaskMText\\MaskMText.dll")
  (princ "\nMaskMText plugin loaded - Type BB to run.")
  (princ)
)
