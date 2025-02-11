import withAdminAuth from '../../HOC/withAdminAuth';


export default withAdminAuth(function AuthorizationTestAdmin() {

    return (
        <div>
            <p>can be access by authorized!</p>
        </div>
    );
})
