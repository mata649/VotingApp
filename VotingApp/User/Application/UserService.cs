
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Pool.Domain;
using VotingApp.User.Domain;
using VotingApp.User.Domain.DTO;

namespace VotingApp.User.Application;
public class UserService : IUserService
{
    private readonly UnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public UserService(UnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    private string HashPassword(string password)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private bool VerifyPassword(string password, string hash)
    {
        string hashedPassword = HashPassword(password);
        return hash.Equals(hashedPassword);
    }


    public IResponse Login(LoginUserDTO loginUserDTO)
    {
        try
        {
            UserEntity user = loginUserDTO.ToUser();
            UserEntity? userFound = _unitOfWork.UserRepository.GetByEmail(user.Email);

            if (userFound is null) return new ResponseFailure("Invalid credentials", 401);

            if (!VerifyPassword(user.Password, userFound.Password)) return new ResponseFailure("Invalid credentials", 401);

            return new ResponseSuccess(ResponseUserDTO.FromUser(userFound), 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }

    public IResponse ChangePassword(ChangePasswordDTO changePasswordDTO)
    {
        try
        {
            UserEntity user = changePasswordDTO.ToUser();

            UserEntity? userFound = _unitOfWork.UserRepository.GetById(user.ID);

            if (userFound is null) return new ResponseFailure($"User was not found", 404);
            if (userFound.ID != changePasswordDTO.CurrentUserID) return new ResponseFailure("Unauthorized", 401);
            if (!VerifyPassword(user.Password, userFound.Password)) return new ResponseFailure("Password incorrect", 401);

            userFound.Password = HashPassword(changePasswordDTO.NewPassword);
            _unitOfWork.UserRepository.Update(userFound);
            _unitOfWork.Save();
            return new ResponseSuccess(ResponseUserDTO.FromUser(userFound), 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }

    public IResponse Create(CreateUserDTO createDTO)
    {
        try
        {
            UserEntity user = createDTO.ToEntity();
            if (_unitOfWork.UserRepository.GetByEmail(user.Email) is not null) return new ResponseFailure("The email is already registered", 409);
            user.Password = HashPassword(user.Password);
            _unitOfWork.UserRepository.Create(user);
            _unitOfWork.Save();
            return new ResponseSuccess(ResponseUserDTO.FromUser(user), 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }

    public IResponse Update(UpdateUserDTO updateDTO)
    {
        try
        {
            UserEntity user = updateDTO.ToEntity();
            UserEntity? userFound = _unitOfWork.UserRepository.GetById(user.ID);
            if (userFound is null) return new ResponseFailure($"User was not found", 404);
            if (userFound.ID != updateDTO.CurrentUserID) return new ResponseFailure("Unauthorized", 401);
            userFound.Name = user.Name;
            userFound.Email = user.Email;
            _unitOfWork.UserRepository.Update(userFound);
            _unitOfWork.Save();
            return new ResponseSuccess(ResponseUserDTO.FromUser(userFound), 200);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }
    public IResponse Get(UserFiltersDTO userFiltersDTO)
    {
        try
        {
            (List<UserEntity> results, ushort currentPage, ushort totalPages) = _unitOfWork.UserRepository.Get(userFiltersDTO.Filters, userFiltersDTO.Pagination);


            return new ResponseSuccess(
                new Results<ResponseUserDTO>()
                {
                    CurrentPage = currentPage,
                    TotalPages = totalPages,
                    Data = results.Select(ResponseUserDTO.FromUser)
                }
                , 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }

    public virtual IResponse Delete(DeleteDTO deleteDTO)
    {
        try
        {
            UserEntity? userFound = _unitOfWork.UserRepository.GetById(deleteDTO.ID);

            if (userFound is null) return new ResponseFailure($"User was not found ", 404);
            if (userFound.ID != deleteDTO.CurrentUserID) return new ResponseFailure("Unauthorized", 401);

            _unitOfWork.UserRepository.Delete(deleteDTO.ID);
            _unitOfWork.Save();
            return new ResponseSuccess(ResponseUserDTO.FromUser(userFound), 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }




    public virtual IResponse GetById(GetByIDDTO getByIDDTO)
    {
        try
        {
            UserEntity? userFound = _unitOfWork.UserRepository.GetById(getByIDDTO.ID);
            if (userFound is null) return new ResponseFailure($"user was not found", 404);

            return new ResponseSuccess(ResponseUserDTO.FromUser(userFound), 200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
            return new ResponseFailure("Internal Error", 500);
        }
    }
}
