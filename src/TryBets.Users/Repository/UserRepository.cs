using TryBets.Users.Models;
using TryBets.Users.DTO;

namespace TryBets.Users.Repository;

public class UserRepository : IUserRepository
{
    protected readonly ITryBetsContext _context;
    public UserRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public User Post(User user)
    {
        var userFound = _context.Users.Where(usuario => usuario.Email == user.Email);

        if (userFound.Count() > 0) {
            return null!;
        }

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }
    public User Login(AuthDTORequest login)
    {
       var userToLogin = _context.Users.Where(user => user.Email == login.Email && user.Password == login.Password);

       if (userToLogin.Count() == 0) {
        return null!;
       }

       return userToLogin.First();
    }
}