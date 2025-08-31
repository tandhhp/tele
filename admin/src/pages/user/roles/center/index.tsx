import WfUpload from "@/components/file-explorer/upload";
import { apiDosOptions, apiLockUser, apiRoleOptions, apiSetPassword, apiSmOptions, apiTmOptions, apiUnLockUser, apiUserByRoleOptions, apiUserUpdate, createEmployee, deleteUser, getUserInRoles } from "@/services/user";
import { BRANCH_OPTIONS, UserStatus } from "@/utils/constants";
import { DeleteOutlined, EditOutlined, LockOutlined, ManOutlined, MoreOutlined, UserAddOutlined, UserOutlined, WomanOutlined } from "@ant-design/icons";
import { ActionType, ModalForm, PageContainer, ProColumns, ProFormDatePicker, ProFormInstance, ProFormSelect, ProFormText, ProTable } from "@ant-design/pro-components"
import { useAccess, useParams } from "@umijs/max";
import { Button, Col, Dropdown, Popconfirm, Row, Select, Space, Tag, message } from "antd";
import dayjs from "dayjs";
import { useEffect, useRef, useState } from "react";

const RoleCenter: React.FC = () => {

    const { id } = useParams();
    const actionRef = useRef<ActionType>();
    const [open, setOpen] = useState<boolean>(false);
    const formRef = useRef<ProFormInstance>();
    const [user, setUser] = useState<any>();
    const [openUpload, setOpenUpload] = useState<boolean>(false);
    const [sm, setSm] = useState<any[]>([]);
    const access = useAccess();
    const [teams, setTeams] = useState<any[]>([]);
    const [teleTeams, setTeleTeams] = useState<any[]>([]);
    const [dotOptions, setDotOptions] = useState<any[]>([]);
    const [openChangePassword, setOpenChangePassword] = useState<boolean>(false);
    const [status, setStatus] = useState<number>(UserStatus.Working);

    useEffect(() => {
        if (user) {
            if (user.dosId) {
                apiSmOptions(user.dosId).then(response => {
                    setSm(response);
                })
            }
            formRef.current?.setFields([
                {
                    name: 'id',
                    value: user.id
                },
                {
                    name: 'userName',
                    value: user.userName
                },
                {
                    name: 'name',
                    value: user.name
                },
                {
                    name: 'email',
                    value: user.email
                },
                {
                    name: 'phoneNumber',
                    value: user.phoneNumber
                },
                {
                    name: 'gender',
                    value: user.gender
                },
                {
                    name: 'dateOfBirth',
                    value: user.dateOfBirth
                },
                {
                    name: 'avatar',
                    value: user.avatar
                },
                {
                    name: 'address',
                    value: user.address
                },
                {
                    name: 'tier',
                    value: user.tier
                },
                {
                    name: 'identityNumber',
                    value: user.identityNumber
                },
                {
                    name: 'identityDate',
                    value: user.identityDate
                },
                {
                    name: 'identityAddress',
                    value: user.identityAddress
                },
                {
                    name: 'role',
                    value: id
                },
                {
                    name: 'dosId',
                    value: user.dosId
                },
                {
                    name: 'smId',
                    value: user.smId
                },
                {
                    name: 'branch',
                    value: user.branch
                },
                {
                    name: 'tmId',
                    value: user.tmId
                },
                {
                    name: 'trainerId',
                    value: user.trainerId
                },
                {
                    name: 'dotId',
                    value: user.dotId
                }
            ]);

        }

        apiSmOptions().then(response => setTeams(response));
        apiTmOptions().then(response => setTeleTeams(response));
        apiUserByRoleOptions('dot').then(response => setDotOptions(response));
    }, [user]);

    const onFinish = async (values: any) => {
        if (values.dateOfBirth) {
            values.dateOfBirth = dayjs(values.dateOfBirth).format('YYYY-MM-DD');
        }
        if (values.identityDate) {
            values.identityDate = dayjs(values.identityDate).format('YYYY-MM-DD');
        }
        if (values.id) {
            const response = await apiUserUpdate(values);
            if (response.succeeded) {
                message.success('Cập nhật thành công!');
                setOpen(false);
                actionRef.current?.reload();
                setUser(null);
                formRef.current?.resetFields();
            }
            return;
        }
        const response = await createEmployee(values);
        if (response.succeeded) {
            message.success('Đã lưu');
            setOpen(false);
            actionRef.current?.reload();
        } else {
            message.error(response.errors[0].description)
        }
    };

    const onConfirm = async (id?: string) => {
        const response = await deleteUser(id);
        if (response.succeeded) {
            message.success('Deleted');
            actionRef.current?.reload();
        }
    }

    const onLock = async (id: string) => {
        await apiLockUser(id);
        message.success('Thành công!');
        actionRef.current?.reload();
    }

    const onUnLock = async (id: string) => {
        await apiUnLockUser(id);
        message.success('Thành công!');
        actionRef.current?.reload();
    }

    const columns: ProColumns<any>[] = [
        {
            title: '#',
            valueType: 'indexBorder',
            width: 30
        },
        {
            title: <UserOutlined />,
            dataIndex: 'avatar',
            search: false,
            valueType: 'avatar',
            width: 30
        },
        {
            title: 'Họ & Tên',
            dataIndex: 'name',
            search: false,
            render: (dom, entity) => (
                <div>
                    {entity.gender === true && (<ManOutlined className='text-blue-500' />)}{entity.gender === false && (<WomanOutlined className='text-red-500' />)} {dom}
                </div>
            )
        },
        {
            title: 'Tài khoản',
            dataIndex: 'userName',
            search: false
        },
        {
            title: 'Email',
            dataIndex: 'email',
            ellipsis: true,
            width: 200
        },
        {
            title: 'Số điện thoại',
            dataIndex: 'phoneNumber',
            width: 110
        },
        {
            title: 'Ngày sinh',
            dataIndex: 'dateOfBirth',
            valueType: 'date',
            width: 100,
            search: false,
            render: (_, entity) => entity.dateOfBirth ? dayjs(entity.dateOfBirth).format('DD-MM-YYYY') : '-'
        },
        {
            title: 'Địa chỉ',
            dataIndex: 'address',
            search: false,
            ellipsis: true
        },
        {
            title: 'CCCD',
            dataIndex: 'identityNumber',
            search: false
        },
        {
            title: 'Ngày cấp',
            dataIndex: 'identityDate',
            valueType: 'date',
            width: 100,
            search: false
        },
        {
            title: 'Nơi cấp',
            dataIndex: 'identityAddress',
            search: false
        },
        {
            title: 'Team',
            dataIndex: 'smId',
            valueType: 'select',
            fieldProps: {
                options: teams,
                showSearch: true
            },
            hideInTable: id !== 'sales',
            hideInSearch: id !== 'sales'
        },
        {
            title: 'Team',
            dataIndex: 'tmId',
            valueType: 'select',
            fieldProps: {
                options: teleTeams,
                showSearch: true
            },
            hideInTable: id !== 'Telesale',
            hideInSearch: id !== 'Telesale'
        },
        {
            title: 'Team',
            dataIndex: 'dotId',
            valueType: 'select',
            fieldProps: {
                options: dotOptions,
                showSearch: true
            },
            hideInTable: id !== 'TelesaleManager',
            hideInSearch: id !== 'TelesaleManager'
        },
        {
            title: 'Trạng thái',
            dataIndex: 'status',
            search: false,
            width: 80,
            valueEnum: {
                0: <Tag color="processing">Đang làm</Tag>,
                1: <Tag color="error">Đã nghỉ</Tag>
            }
        },
        {
            title: 'Tác vụ',
            valueType: 'option',
            render: (dom, entity) => [
                <Button size='small' icon={<EditOutlined />} type='primary' key="edit" hidden={!access.hr && !access.dos} onClick={() => {
                    setUser(entity);
                    setOpen(true);
                }}></Button>,
                <Dropdown key="more" menu={{
                    items: [
                        {
                            key: 'change-password',
                            label: 'Đặt lại mật khẩu',
                            disabled: !access.canAdmin && !access.hr
                        }
                    ],
                    onClick: (info) => {
                        setUser(entity);
                        if (info.key === 'change-password') {
                            setOpenChangePassword(true);
                            return;
                        }
                    }
                }}>
                    <Button type="dashed" icon={<MoreOutlined />} size="small" />
                </Dropdown>,
                <Popconfirm title="Mở khóa tài khoản?" key="unlock" onConfirm={() => onUnLock(entity.id)}>
                    <Button icon={<LockOutlined />} size='small' hidden={!access.hr || entity.status === 0} />
                </Popconfirm>,
                <Popconfirm title="Khóa tài khoản?" key={23} onConfirm={() => onLock(entity.id)}>
                    <Button type="primary" icon={<LockOutlined />} size='small' danger hidden={!access.hr || entity.status === 1} />
                </Popconfirm>,
                <Popconfirm title="Xác nhận xóa?" key={2} onConfirm={() => onConfirm(entity.id)}>
                    <Button type="primary" icon={<DeleteOutlined />} size='small' danger hidden={!access.canAdmin} />
                </Popconfirm>
            ],
            width: 80
        }
    ]

    return (
        <PageContainer extra={<Button type='primary' icon={<UserAddOutlined />} onClick={() => setOpen(true)} hidden={!access.canCreateEmployee}>Thêm nhân viên</Button>}>
            <ProTable
                headerTitle={(
                    <Space>
                        Trạng thái
                        <Select defaultValue={UserStatus.Working} onChange={(value) => {
                            setStatus(value);
                            actionRef.current?.reload();
                        }} options={[
                            {
                                label: 'Đang làm',
                                value: UserStatus.Working
                            },
                            {
                                label: 'Đã nghỉ',
                                value: UserStatus.Leave
                            }
                        ]} />
                    </Space>
                )}
                scroll={{
                    x: true
                }}
                search={{
                    layout: 'vertical'
                }}
                request={(params) => getUserInRoles({
                    ...params,
                    status: status
                }, id)} columns={columns}
                actionRef={actionRef} />
            <ModalForm
                formRef={formRef}
                open={open}
                onOpenChange={(visible) => {
                    if (!visible) {
                        setUser(null);
                        formRef.current?.resetFields();
                    }
                    setOpen(visible);
                }}
                title="Quản lý nhân viên"
                onFinish={onFinish}
            >
                <ProFormText name="id" hidden />
                <Row gutter={16}>
                    <Col md={8} xs={12}>
                        <ProFormText name="userName" label="Tài khoản" fieldProps={{
                            autoComplete: 'off'
                        }} disabled={user} rules={[
                            {
                                required: true
                            }
                        ]} />
                    </Col>
                    <Col md={8}>
                        <ProFormText name="name" label="Họ và tên" rules={[
                            {
                                required: true
                            }
                        ]} />
                    </Col>
                    <Col md={8} xs={12}>
                        <ProFormText
                            name="email"
                            label="Email"
                            rules={[
                                {
                                    required: true,
                                },
                                {
                                    type: 'email'
                                }
                            ]}
                        />
                    </Col>
                    <Col md={6} xs={12}>
                        <ProFormText name="phoneNumber" label="Số điện thoại" fieldProps={{
                            autoComplete: 'off'
                        }} />
                    </Col>
                    <Col md={8} xs={12}>
                        <ProFormText.Password
                            fieldProps={{
                                autoComplete: 'off'
                            }}
                            disabled={user}
                            name="password"
                            label="Mật khẩu"
                            rules={[
                                {
                                    required: !user,
                                },
                            ]}
                        />
                    </Col>
                    <Col md={4} xs={12}>
                        <ProFormSelect label="Giới tính" name="gender" options={[
                            {
                                label: 'Nam',
                                value: true as any
                            },
                            {
                                label: 'Nữ',
                                value: false as any
                            },
                            {
                                label: 'Khác',
                                value: null
                            }
                        ]} />
                    </Col>
                    <Col md={6} xs={12}>
                        <ProFormDatePicker name="dateOfBirth" label="Ngày sinh" width="lg" fieldProps={{
                            format: {
                                type: 'mask',
                                format: 'DD-MM-YYYY'
                            }
                        }} />
                    </Col>
                    <Col md={8} xs={12}>
                        <ProFormText name="identityNumber" label="CMT/CCCD/Hộ chiếu" />
                    </Col>
                    <Col md={8} xs={12}>
                        <ProFormDatePicker name="identityDate" label="Ngày cấp" width="lg" fieldProps={{
                            format: {
                                type: 'mask',
                                format: 'DD-MM-YYYY'
                            }
                        }} />
                    </Col>
                    <Col md={8} xs={12}>
                        <ProFormText name="identityAddress" label="Nơi cấp" />
                    </Col>
                    <Col md={16} xs={12}>
                        <ProFormText name="address" label="Địa chỉ" />
                    </Col>
                    <Col md={8} xs={12}>
                        <ProFormSelect request={apiRoleOptions} name="role" label="Quyền" initialValue={id} disabled />
                    </Col>
                    <Col md={12} hidden={!(id === 'sm' || id === 'sales')}>
                        <ProFormSelect request={apiDosOptions}
                            fieldProps={{
                                onChange: (value: string) => {
                                    apiSmOptions(value).then(response => {
                                        setSm(response);
                                    });
                                }
                            }}
                            name="dosId" label="Giám đốc quan hệ khách hàng" />
                    </Col>
                    <Col md={12} xs={12} hidden={id !== 'sales'}>
                        <ProFormSelect options={sm} name="smId" label="Quản lí quan hệ khách hàng" showSearch />
                    </Col>
                    <Col md={12} xs={12} hidden={id !== 'TelesaleManager'}>
                        <ProFormSelect request={() => apiUserByRoleOptions('dot')} name="dotId" label="Director of Tele" showSearch />
                    </Col>
                    <Col md={12} xs={12}>
                        <ProFormSelect request={() => apiUserByRoleOptions('Trainer')} name="trainerId" label="Trainer" showSearch />
                    </Col>
                    <Col md={12} xs={12}>
                        <ProFormSelect initialValue={0} options={BRANCH_OPTIONS} name="branch" label="Chi nhánh" />
                    </Col>
                    <Col md={12} hidden={id !== 'Telesale'} xs={12}>
                        <ProFormSelect request={apiTmOptions} name="tmId" label="Tele Manager" />
                    </Col>
                </Row>

            </ModalForm>
            <WfUpload open={openUpload} onCancel={() => setOpenUpload(false)} onFinish={(value: string) => {
                formRef.current?.setFieldValue('avatar', value)
            }} />
            <ModalForm open={openChangePassword} onOpenChange={setOpenChangePassword} title={`Đặt lại mật khẩu ${user?.name}`} onFinish={async (values) => {
                values.userId = user?.id;
                await apiSetPassword(values);
                message.success('Thành công!');
                setOpenChangePassword(false);
            }}>
                <ProFormText name="password" label="Mật khẩu mới" rules={[
                    {
                        required: true
                    }
                ]} />
            </ModalForm>
        </PageContainer>
    )
}

export default RoleCenter